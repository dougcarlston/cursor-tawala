package com.tawala.project;

import java.util.List;

public interface ParagraphDisplayStrategy extends FormRenderable {

	List<FormRenderable> getContents();
}
