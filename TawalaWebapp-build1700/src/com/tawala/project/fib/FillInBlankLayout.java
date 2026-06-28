package com.tawala.project.fib;

import com.tawala.project.Blank;
import com.tawala.project.FillInBlank;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;

public interface FillInBlankLayout {
	Html render(FillInBlank fib, ExecutionContext executionContext);
	Html render(Blank blank, ExecutionContext executionContext);
}
