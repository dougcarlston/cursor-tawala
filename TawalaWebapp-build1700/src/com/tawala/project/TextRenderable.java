package com.tawala.project;

import com.tawala.project.commands.ExecutionContext;

public interface TextRenderable {
    void appendTo(StringBuilder result, ExecutionContext context);
}
