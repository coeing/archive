package controllers;

import models.user.User;
import play.mvc.Before;
import play.mvc.Controller;
import services.UserService;
import controllers.Secure.Security;

public class BaseController extends Controller {
    private static UserService userService = new UserService();

    @Before
    static void setConnectedUser() {
	if (Security.isConnected()) {
	    User connectedUser = userService.findByUsername(Security.connected());
	    renderArgs.put("user", connectedUser);
	}
    }

    @Before
    public static void csrfProtection() {
	if (request.method == "POST" && !request.isAjax()) {
	    checkAuthenticity();
	}
    }

    protected static User connectedUser() {
	return (User) renderArgs.get("user");
    }

    public BaseController() {
	super();
    }
}