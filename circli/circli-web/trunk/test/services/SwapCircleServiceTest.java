package services;

import junit.framework.Assert;
import models.circle.SwapCircle;
import models.user.User;
import models.user.item.WantedItem;

import org.junit.Before;
import org.junit.Test;

import play.test.Fixtures;
import play.test.UnitTest;

public class SwapCircleServiceTest extends UnitTest {
    private final SwapCircleService service = new SwapCircleService();

    private WantedItem getFirstWantedItemOfUser(String email) {
	return User.find("byEmail", email).<User> first().wantedItems.iterator().next();
    }

    @Before
    public void setUp() {
	Fixtures.deleteDatabase();
	Fixtures.loadModels("data-users.yml", "data-books.yml");
    }

    @Test
    public void testAlternativePathIsFound() {
	Fixtures.loadModels("data-circle-alternative-paths.yml");
	SwapCircle swapCircle = service.getSwapCircle(getFirstWantedItemOfAdam());
	Assert.assertNotNull(swapCircle);
	Assert.assertEquals("expecting circle of size 3", 3, swapCircle.getSteps().size());
	Assert.assertTrue(swapCircle.isComplete());
    }

    private WantedItem getFirstWantedItemOfAdam() {
	return getFirstWantedItemOfUser("adam@circ.li");
    }

    @Test
    public void testCircleWithBackreferenceDoesNotLeadToInfiniteLoop() {
	Fixtures.loadModels("data-circle-backref.yml");
	SwapCircle swapCircle = service.getSwapCircle(getFirstWantedItemOfAdam());
	Assert.assertNull(swapCircle);
    }

    @Test
    public void testDirectSwapCircleIsFound() {
	Fixtures.loadModels("data-circle-size-2.yml");
	SwapCircle swapCircle = service.getSwapCircle(getFirstWantedItemOfAdam());
	Assert.assertNotNull(swapCircle);
	Assert.assertEquals("expecting circle of size 2", 2, swapCircle.getSteps().size());

	// try it the other way round
	WantedItem firstWantedItemOfBerta = getFirstWantedItemOfUser("berta@circ.li");
	SwapCircle swapCircle2 = service.getSwapCircle(firstWantedItemOfBerta);
	Assert.assertNotNull(swapCircle2);
	Assert.assertEquals("expecting circle of size 2", 2, swapCircle2.getSteps().size());
    }

    @Test
    public void testItemsAlreadyInCirclesAreIgnored() {
	Assert.fail("not yet implemented.");
    }

    @Test
    public void testLargerCircleIsFound() {
	Fixtures.loadModels("data-circle-size-3.yml");
	SwapCircle swapCircle = service.getSwapCircle(getFirstWantedItemOfAdam());
	Assert.assertNotNull(swapCircle);
	Assert.assertEquals("expecting circle of size 3", 3, swapCircle.getSteps().size());
    }

    @Test
    public void testNonCompleteableCircleIsIgnored() {
	Fixtures.loadModels("data-circle-broken.yml");
	SwapCircle swapCircle = service.getSwapCircle(getFirstWantedItemOfAdam());
	Assert.assertNull(swapCircle);
    }
}