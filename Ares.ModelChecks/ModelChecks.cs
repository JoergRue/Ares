/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ares.Data;

namespace Ares.ModelInfo
{
    public enum CheckType
    {
        File,
        Key,
        Logic,
        Reference
    }

    public class ModelError
    {
        public enum ErrorSeverity { Warning, Error }

        public Object Element { get; internal set; }
        public String Message { get; internal set; }
        public ErrorSeverity Severity { get; internal set; }

        internal ModelError(ErrorSeverity severity, String message, Object element)
        {
            Element = element;
            Message = message;
            Severity = severity;
        }
    }

    public interface IModelErrors
    {
        void AddError(CheckType checkType, ModelError error);
    }

    public interface IModelCheck
    {
        CheckType CheckType { get; }
        void DoChecks(IProject project, IModelErrors errors);
    }

    public abstract class ModelCheck : IModelCheck
    {
        public CheckType CheckType { get; private set; }

        public abstract void DoChecks(IProject project, IModelErrors errors);

        protected ModelCheck(CheckType checkType)
        {
            CheckType = checkType;
        }

        protected void AddError(IModelErrors errors, ModelError.ErrorSeverity severity, String message, Object element)
        {
            errors.AddError(CheckType, new ModelError(severity, message, element));
        }
    }

    public class ModelChecks : IModelErrors
    {
        public static ModelChecks Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new ModelChecks();
                }
                return sInstance;
            }
        }

        private static ModelChecks sInstance;

        private ModelChecks()
        {
            m_Errors = new Dictionary<CheckType, List<ModelError>>();
            m_ErrorsByElement = new Dictionary<Object, List<ModelError>>();
            m_ModelChecks = new Dictionary<CheckType, IModelCheck>();
			
            AddCheck(new FileChecks());
            AddCheck(new ReferenceChecks());
        }

        public event EventHandler<EventArgs> ErrorsUpdated;

        private Dictionary<CheckType, List<ModelError>> m_Errors;
        private Dictionary<Object, List<ModelError>> m_ErrorsByElement;
        private Dictionary<CheckType, IModelCheck> m_ModelChecks;

        public void AddError(CheckType checkType, ModelError error)
        {
            m_Errors[checkType].Add(error);
            if (!m_ErrorsByElement.ContainsKey(error.Element))
            {
                m_ErrorsByElement[error.Element] = new List<ModelError>();
            }
            m_ErrorsByElement[error.Element].Add(error);
        }

        private void RemoveError(ModelError error)
        {
            m_ErrorsByElement[error.Element].Remove(error);
            if (m_ErrorsByElement[error.Element].Count == 0)
            {
                m_ErrorsByElement.Remove(error.Element);
            }
        }

        public void AddCheck(IModelCheck check)
        {
            m_ModelChecks[check.CheckType] = check;
        }

        public IList<ModelError> GetErrorsForElement(Object element)
        {
            if (m_ErrorsByElement.ContainsKey(element))
                return m_ErrorsByElement[element];
            else
                return new List<ModelError>();
        }

        public IEnumerable<ModelError> GetAllErrors()
        {
            foreach (List<ModelError> errors in m_Errors.Values)
                foreach (ModelError error in errors)
                    yield return error;
        }

        public int GetErrorCount()
        {
            int result = 0;
            foreach (List<ModelError> errors in m_Errors.Values)
                result += errors.Count;
            return result;
        }

        public void CheckAll(IProject project)
        {
            foreach (CheckType type in m_ModelChecks.Keys)
            {
                DoCheck(type, project);
            }
            if (ErrorsUpdated != null)
                ErrorsUpdated(this, new EventArgs());
        }

        public void Check(CheckType checkType, IProject project)
        {
            DoCheck(checkType, project);
            if (ErrorsUpdated != null)
                ErrorsUpdated(this, new EventArgs());
        }

        private void DoCheck(CheckType checkType, IProject project)
        {
            if (!m_ModelChecks.ContainsKey(checkType))
                return;
            if (!m_Errors.ContainsKey(checkType))
                m_Errors[checkType] = new List<ModelError>();
            foreach (ModelError error in m_Errors[checkType])
            {
                RemoveError(error);
            }
            m_Errors[checkType].Clear();
            if (project != null)
            {
                m_ModelChecks[checkType].DoChecks(project, this);
            }
        }

        public void AdaptHiddenTags(IProject project)
        {
            if (project == null)
                return;
            HashSet<int> hiddenCategories = project.GetHiddenTagCategories();
            HashSet<int> hiddenTags = project.GetHiddenTags();
            if (hiddenCategories.Count == 0 && hiddenTags.Count == 0)
                return;

            try
            {
                int languageId = project.TagLanguageId;
                if (languageId == -1)
                    languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId);

                if (hiddenCategories.Count > 0)
                {
                    var existingCategories = dbRead.GetAllCategories();
                    HashSet<int> existing = new HashSet<int>();
                    foreach (var category in existingCategories)
                        existing.Add(category.Id);
                    foreach (int catId in hiddenCategories)
                    {
                        if (!existing.Contains(catId))
                            project.SetTagCategoryHidden(catId, false);
                    }
                }

                if (hiddenTags.Count > 0)
                {
                    var tagInfos = dbRead.GetTagInfos(hiddenTags);
                    HashSet<int> existing = new HashSet<int>();
                    foreach (var tag in tagInfos)
                        existing.Add(tag.Id);
                    foreach (int tagId in hiddenTags)
                    {
                        if (!existing.Contains(tagId))
                            project.SetTagHidden(tagId, false);
                    }
                }
            }
            catch (Ares.Tags.TagsDbException)
            {
                // ignore here
            }
        }
    }

}
