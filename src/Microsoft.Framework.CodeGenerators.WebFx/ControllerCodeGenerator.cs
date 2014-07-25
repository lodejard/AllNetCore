﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Framework.CodeGeneration;
using Microsoft.Framework.CodeGeneration.CommandLine;
using Microsoft.Framework.CodeGeneration.EntityFramework;
using Microsoft.Framework.Runtime;

namespace Microsoft.Framework.CodeGenerators.WebFx
{
    [Alias("controller")]
    public class ControllerCodeGenerator : ICodeGenerator
    {

        private readonly IModelTypesLocator _modelTypesLocator;
        private readonly IEntityFrameworkService _entityFrameworkService;
        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly ICodeGeneratorActionsService _codeGeneratorActionsService;
        private readonly ILibraryManager _libraryManager;

        private readonly List<string> _views = new List<string>()
        {
            "Create",
            "Edit",
            "Details",
            "Delete",
            "List"
        };

        public ControllerCodeGenerator(
            [NotNull]ILibraryManager libraryManager,
            [NotNull]IApplicationEnvironment environment,
            [NotNull]IModelTypesLocator modelTypesLocator,
            [NotNull]IEntityFrameworkService entityFrameworkService,
            [NotNull]ICodeGeneratorActionsService codeGeneratorActionsService)
        {
            _libraryManager = libraryManager;
            _applicationEnvironment = environment;
            _codeGeneratorActionsService = codeGeneratorActionsService;
            _modelTypesLocator = modelTypesLocator;
            _entityFrameworkService = entityFrameworkService;
        }

        public virtual IEnumerable<string> TemplateFolders
        {
            get
            {
                return TemplateFoldersUtil.GetTemplateFolders(
                    containingProject: "Microsoft.Framework.CodeGenerators.WebFx",
                    libraryManager: _libraryManager,
                    appEnvironment: _applicationEnvironment);
            }
        }

        public async Task GenerateCode([NotNull]ControllerGeneratorModel controllerGeneratorModel)
        {
            // Validate model
            string validationMessage;
            ITypeSymbol model, dataContext;

            if (!ValidationUtil.TryValidateType(controllerGeneratorModel.ModelClass, "model", _modelTypesLocator, out model, out validationMessage) ||
                !ValidationUtil.TryValidateType(controllerGeneratorModel.DataContextClass, "dataContext", _modelTypesLocator, out dataContext, out validationMessage))
            {
                throw new Exception(validationMessage);
            }

            if (string.IsNullOrEmpty(controllerGeneratorModel.ControllerName))
            {
                //Todo: Pluralize model name
                controllerGeneratorModel.ControllerName = model.Name + Constants.ControllerSuffix;
            }

            // Validation successful
            Contract.Assert(model != null, "Validation succeded but model type not set");
            Contract.Assert(dataContext != null, "Validation succeded but DataContext type not set");

            var templateName = "ControllerWithContext.cshtml";

            var outputPath = Path.Combine(
                _applicationEnvironment.ApplicationBasePath,
                Constants.ControllersFolderName,
                controllerGeneratorModel.ControllerName + ".cs");

            if (File.Exists(outputPath) && !controllerGeneratorModel.Force)
            {
                throw new Exception(string.Format(
                    CultureInfo.CurrentCulture,
                    "View file {0} exists, use -f option to overwrite",
                    outputPath));
            }

            var dbContextFullName = dataContext.FullNameForSymbol();
            var modelTypeFullName = model.FullNameForSymbol();

            var modelMetadata = _entityFrameworkService.GetModelMetadata(
                dbContextFullName,
                modelTypeFullName);

            var templateModel = new ControllerGeneratorTemplateModel(model, dataContext)
            {
                ControllerName = controllerGeneratorModel.ControllerName,
                AreaName = string.Empty, //ToDo
                UseAsync = controllerGeneratorModel.UseAsync,
                ModelMetadata = modelMetadata
            };

            await _codeGeneratorActionsService.AddFileFromTemplateAsync(outputPath, templateName, TemplateFolders, templateModel);

            if (controllerGeneratorModel.GenerateViews)
            {
                foreach (var viewTemplate in _views)
                {
                    var viewName = viewTemplate == "List" ? "Index" : viewTemplate;
                    // ToDo: This is duplicated from ViewGenerator.
                    var viewTemplateModel = new ViewGeneratorTemplateModel()
                    {
                        ViewDataTypeName = modelTypeFullName,
                        ViewName = viewName,
                        LayoutPageFile = controllerGeneratorModel.LayoutPage,
                        IsLayoutPageSelected = controllerGeneratorModel.UseLayout,
                        IsPartialView = false,
                        ReferenceScriptLibraries = controllerGeneratorModel.ReferenceScriptLibraries,
                        ModelMetadata = modelMetadata,
                        JQueryVersion = "1.10.2"
                    };

                    var viewOutputPath = Path.Combine(
                        _applicationEnvironment.ApplicationBasePath,
                        Constants.ViewsFolderName,
                        model.Name,
                        viewName + ".cshtml");

                    await _codeGeneratorActionsService.AddFileFromTemplateAsync(viewOutputPath,
                        viewTemplate + ".cshtml", TemplateFolders, viewTemplateModel);
                }
            }
        }
    }
}