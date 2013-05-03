package secure;

import java.util.List;

import notifiers.Mails;

import models.user.User;
import play.Logger;
import play.db.jpa.JPA;
import play.mvc.Scope;
import services.UserService;

import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

public class FacebookOAuthHandler {
    private static UserService userService = new UserService();

    public static void facebookOAuthCallback(JsonObject data) {
	if (Logger.isDebugEnabled()) {
	    Logger.debug("Got Facebook OAuth callback with data: %s", data);
	}

	JsonElement emailElement = data.get("email");
	if (emailElement == null || emailElement.getAsString().isEmpty()) {
	    Logger.warn("Could not extract email from data, unable to recognize user!");
	    return;
	}

	// try to find the user in our database and add him when not found
	String email = emailElement.getAsString();
	User user = userService.findByEmail(email);
	if (user == null) {
	    user = User.createFromFacebookData(data);
	    if (Logger.isDebugEnabled()) {
		Logger.debug("New user created: %s", user.toInspectString());
	    }
	    user.save();

	    // welcome the user
	    Mails.welcome(user);
	}

	// TODO maybe sync our stored user with the data from Facebook

	// mark user as logged in by saving the username into the session
	Scope.Session.current().put("username", email);
    }
}
