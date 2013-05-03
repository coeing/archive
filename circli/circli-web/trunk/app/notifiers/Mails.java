package notifiers;

import models.user.User;
import play.i18n.Lang;
import play.mvc.Mailer;

public class Mails extends Mailer {
    public static void welcome(User user) {
	setSubject("Welcome %s", user.getFullName());
	addRecipient(user.email);
	setFrom("circ.li <info@circ.li>");
	send("Mails/welcome." + Lang.get(), user);
    }
}