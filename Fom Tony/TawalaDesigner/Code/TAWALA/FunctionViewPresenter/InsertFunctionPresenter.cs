// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;

namespace Tawala.Functions.ViewPresenter
{
    public class InsertFunctionPresenter : IInsertFunctionPresenter
    {
        private readonly IFunctionRepository functionRepository;
        private readonly IInsertFunctionView view;

        public InsertFunctionPresenter(IInsertFunctionView view)
        {
            this.view = view;
            functionRepository = FunctionLoader.Repository;

            this.view.Setup(functionRepository.Categories);
        }
    }
}