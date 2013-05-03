package services;

import java.util.Iterator;
import java.util.List;

import models.circle.SwapCircle;
import models.circle.SwapCircleStep;
import models.user.User;
import models.user.item.OwnedItem;
import models.user.item.WantedItem;

import org.apache.log4j.Logger;

public class SwapCircleService {

    private final static Logger LOG = Logger.getLogger(SwapCircleService.class);

    /**
     * Tries to find a swap circle for the given wanted item.
     * 
     * @param wantedItemOfRootUser
     *            The item that the user we start with wants to have.
     * @return A fully initialized swap circle or <code>null</code> if none could be found.
     */
    public SwapCircle getSwapCircle(WantedItem wantedItemOfRootUser) {
	return getSwapCircle(wantedItemOfRootUser, wantedItemOfRootUser, null);
    }

    private SwapCircle getSwapCircle(WantedItem wantedItemOfRootUser, WantedItem wantedItemOfFirstUser,
	    SwapCircle swapCircle) {
	// find the owned items that match the wanted item, do not belong to the same user
	// (such a constellation should not be allowed, but just to make sure...) and are not part
	// of a swap circle yet.
	List<OwnedItem> ownedItemsOfOtherUsers = OwnedItem.find(
		"SELECT o FROM OwnedItem o WHERE o.article.uniqueIdentifier = ? " + "AND o.user != ? "
			+ "AND NOT EXISTS (SELECT s FROM SwapCircleStep s WHERE s.ownedItem = o) "
			+ "ORDER BY o.createdAt ASC", wantedItemOfFirstUser.getArticle().getUniqueIdentifier(),
		wantedItemOfFirstUser.getUser()).fetch();

	// remove items that point to a user in our current circle - to prevent infinite loops
	removeItemsThatBelongToUsersContainedInCircle(ownedItemsOfOtherUsers, swapCircle);

	// ...no connection possible because the wanted item is not available!
	if (ownedItemsOfOtherUsers.isEmpty()) {
	    LOG.info(String.format("No owned item found for wanted item: %s, returning null",
		    wantedItemOfFirstUser.toString()));
	    return null;
	}

	// create and try to complete the circle
	if (swapCircle == null) {
	    swapCircle = new SwapCircle();
	}
	for (OwnedItem ownedItemOfOtherUser : ownedItemsOfOtherUsers) {
	    // save the step
	    SwapCircleStep step = new SwapCircleStep(ownedItemOfOtherUser, wantedItemOfFirstUser);
	    swapCircle.addStep(step);

	    // check whether there is a direct connection between this item's user and the root
	    // item's user
	    // TODO we have to exclude the items that are already blocked!
	    User otherUser = ownedItemOfOtherUser.getUser();
	    for (OwnedItem ownedItemOfFirstUser : wantedItemOfRootUser.getUser().ownedItems) {
		for (WantedItem wantedItemOfOtherUser : otherUser.wantedItems) {
		    if (ownedItemOfFirstUser.getArticle().getUniqueIdentifier()
			    .equals(wantedItemOfOtherUser.getArticle().getUniqueIdentifier())) {
			// match -- add steps and return!
			swapCircle.addStep(new SwapCircleStep(ownedItemOfFirstUser, wantedItemOfOtherUser));
			return swapCircle;
		    }
		}
	    }

	    // no match -- check the other user's wanted items by recursive call
	    for (WantedItem wantedItemOfOtherUser : otherUser.wantedItems) {
		SwapCircle subCircle = getSwapCircle(wantedItemOfRootUser, wantedItemOfOtherUser, swapCircle);
		if (subCircle != null) {
		    return subCircle;
		} else {
		    // we have to remove the lastly added step because it lead to a dead end
		    swapCircle.getSteps().remove(step);
		}
	    }
	}

	return null;
    }

    private void removeItemsThatBelongToUsersContainedInCircle(List<OwnedItem> ownedItems, SwapCircle swapCircle) {
	if (swapCircle == null) {
	    return;
	}
	for (Iterator iterator = ownedItems.iterator(); iterator.hasNext();) {
	    OwnedItem ownedItem = (OwnedItem) iterator.next();
	    if (swapCircle.hasOwnedItemWithUser(ownedItem.getUser())) {
		iterator.remove();
	    }
	}
    }
}