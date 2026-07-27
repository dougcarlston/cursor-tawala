package com.tawala.project.library;

import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;


public class CategoryValidator implements Validator {

    public boolean supports(Class clazz) {
        return clazz.equals(Category.class);
    }

    public void validate(Object obj, Errors errors) {
        ValidationUtils.rejectIfEmpty(errors, "name",
                "category.name.empty");
        ValidationUtils.rejectIfEmpty(errors, "description",
                "category.description.empty");
    }
}
