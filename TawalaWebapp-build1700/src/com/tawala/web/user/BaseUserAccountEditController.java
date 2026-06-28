package com.tawala.web.user;

import javax.mail.internet.AddressException;

import org.springframework.util.StringUtils;
import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;

abstract public class BaseUserAccountEditController extends SimpleFormController {
    public BaseUserAccountEditController() {
        setSessionForm(false);
        setValidateOnBinding(false);
        setCommandName("userForm");
        setValidator(new UnvalidatedUserValidator());
    }

    public void validateNonEmptyPasswords(BindException errors, UserForm form) {
        if (! form.getPassword().equals(form.getRepeatedPassword())) {
            errors.reject("user.password.mismatch");
        } else {
            form.getUser().setPassword(form.getPassword());
        }
    }

    public void commonUserValidation(BindException errors, UserForm form) {
        errors.setNestedPath("user");
        getValidator().validate(form.getUser(), errors);
        errors.setNestedPath("");

        if (! StringUtils.hasText(form.getEmailAddress())) {
        	errors.rejectValue("emailAddress", "user.emailaddress.is.empty");
        } else {
            EmailAddress emailAddress = new EmailAddress(form.getEmailAddress());

            try {
                // InternetAddress accepts addresses without @ sign,
                // assuming it's an address on a local machine. We need to force
                // it to include the @, leaving the rest of validation to the
                // InternetAddress class.
                if (emailAddress.toString().indexOf('@') < 0)
                    throw new AddressException();

                if (emailAddress.toString().lastIndexOf('.') < emailAddress.toString().indexOf('@') )
                    throw new AddressException();

                emailAddress.asInternetAddress();
                form.getUser().setEmail(emailAddress);
            } catch (Exception e) {
                errors.rejectValue("emailAddress", "user.emailaddress.invalid");
            }
        }
    }

    public static class UnvalidatedUserValidator implements Validator {
        /*
         * (non-Javadoc)
         * 
         * @see org.springframework.validation.Validator#supports(java.lang.Class)
         */
        public boolean supports(Class clazz) {
            return clazz.equals(User.class);
        }

        public void validate(Object obj, Errors errors) {
            User user = (User) obj;

            if (!StringUtils.hasText(user.getId())) {
                errors.rejectValue("id", "user.id.is.empty");
            }
        }
    }
}
