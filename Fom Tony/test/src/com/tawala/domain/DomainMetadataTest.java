package com.tawala.domain;

import com.tawala.TestCase;

public class DomainMetadataTest extends TestCase {

      public void testMetaDataForProjectLibraryShortDescription() {
          assertEquals(60, DomainMetadata.instance.getLibraryProjectShortDescriptionMaxLength());
      }
}
