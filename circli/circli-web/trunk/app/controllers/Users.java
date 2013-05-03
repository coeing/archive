package controllers;

import models.user.User;
import play.i18n.Messages;
import play.mvc.With;
import services.UserService;

@With(Secure.class)
public class Users extends BaseController {
    private final static UserService userService = new UserService();

    public static void index() {
	// TODO maybe show some statistics here?
    }

    public static void show(Long userId) {
	if (userId.equals(connectedUser().getId())) {
	    edit();
	}
	User user = userService.findById(userId);
	notFoundIfNull(user, "A User with that id is not known!");
	render(user);
    }

    private static void edit() {
	render("@edit", connectedUser());
    }

    public static void save(Long userId, User user) {
	User savedUser = userService.findById(userId);
	if (!connectedUser().equals(savedUser)) {
	    forbidden("You are not allowed to do that.");
	}

	savedUser.firstName = user.firstName;
	savedUser.lastName = user.lastName;
	validation.valid(user);
	if (validation.hasErrors()) {
	    // params.flash(); // put params into flash
	    // validation.keep(); // save errors to next action
	    render("@index");
	}
	savedUser.save();
	flash.success(Messages.get("views.users.index.success"));
	edit();
    }
}
