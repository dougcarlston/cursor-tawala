/**
 * 
 */
package com.tawala.web.user;

import org.springframework.validation.Errors;
import org.springframework.validation.Validator;

import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;

class LoginValidator implements Validator {
    @SuppressWarnings("unchecked")
	public boolean supports(Class clazz) {
        return clazz.equals(LoginForm.class);
    }

    public void validate(Object obj, Errors errors) {
        LoginForm form = (LoginForm) obj;

        User user = WorldInitializer.getDefaultWorld().domain().users().get(
                form.getUserName());
        if (user == null || !user.checkPassword(form.getPassword())) {
            errors.reject("login.failed");
        } else if(user != null && !user.getStatus().isAllowedToLogOn()) {
            errors.reject("login.not.allowed");
        } else {
            WorldInitializer.getDefaultWorld().domain().users().onLogin(user);
            form.setUser(user);
        }
    }
}