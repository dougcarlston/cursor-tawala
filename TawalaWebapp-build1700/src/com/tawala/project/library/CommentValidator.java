package com.tawala.project.library;

import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;

public class CommentValidator implements Validator {

    public boolean supports(Class clazz) {
        return clazz.equals(Comment.class);
    }

    public void validate(Object obj, Errors errors) {
        ValidationUtils.rejectIfEmpty(errors, "text",
                "submitted.project.comment.text.empty");
    }
}
