package services;

import models.user.User;

import org.apache.commons.lang.Validate;

public class UserService {
    public User findByEmail(String email) {
	Validate.notEmpty(email);
	return User.find("byEmail", email).first();
    }

    public User findById(Long userId) {
	Validate.notNull(userId);
	return User.findById(userId);
    }

    public User findByUsername(String username) {
	return findByEmail(username);
    }
}