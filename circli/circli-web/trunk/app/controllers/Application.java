package controllers;

import controllers.Secure.Security;
import models.user.User;
import play.mvc.Before;
import services.UserService;

public class Application extends BaseController {
    public static void index() {
	render();
    }
}