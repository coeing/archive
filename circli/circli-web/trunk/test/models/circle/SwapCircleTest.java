package models.circle;

import java.util.List;

import junit.framework.Assert;
import models.user.User;

import org.junit.Before;
import org.junit.Test;

import play.test.Fixtures;
import play.test.UnitTest;

public class SwapCircleTest extends UnitTest {
    @Before
    public void setUp() {
	Fixtures.deleteDatabase();
	Fixtures.loadModels("data-users.yml", "data-books.yml", "data-circle-size-2.yml");
    }

    @Test
    public void testIsCompleteReturnsFalseForEmptyCircle() {
	SwapCircle emptyCircle = new SwapCircle();
	Assert.assertFalse(emptyCircle.isComplete());
    }

    @Test
    public void testIsCompleteReturnsFalseForIncompleteCircle() {
	SwapCircle incompleteCircle = new SwapCircle();
	List<User> allUsers = User.findAll();
	User userA = allUsers.get(0);
	User userB = allUsers.get(1);
	incompleteCircle.addStep(new SwapCircleStep(userA.ownedItems.iterator().next(), userB.wantedItems.iterator()
		.next()));
	Assert.assertFalse(incompleteCircle.isComplete());
    }

    @Test
    public void testIsCompleteReturnsTrueForCompleteCircle() {
	SwapCircle completeCircle = new SwapCircle();
	List<User> allUsers = User.findAll();
	User userA = allUsers.get(0);
	User userB = allUsers.get(1);
	completeCircle.addStep(new SwapCircleStep(userA.ownedItems.iterator().next(), userB.wantedItems.iterator()
		.next()));
	completeCircle.addStep(new SwapCircleStep(userB.ownedItems.iterator().next(), userA.wantedItems.iterator()
		.next()));
	Assert.assertTrue(completeCircle.isComplete());
    }
}
