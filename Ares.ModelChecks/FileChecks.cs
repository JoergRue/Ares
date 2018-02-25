/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
using System.Threading;
#if ANDROID
using Ares.Settings;
#endif

namespace Ares.ModelInfo
{
    class FileChecks : ModelCheck, IElementVisitor
    {
        public FileChecks()
            : base(CheckType.File)
        {
        }

        private IModelErrors m_ModelErrors;
        private CancellationToken m_Ct;

        public override void DoChecks(Data.IProject project, IModelErrors errors, CancellationToken ct)
        {
            m_ModelErrors = errors;
            m_Ct = ct;
            foreach (IMode mode in project.GetModes())
            {
                foreach (IModeElement element in mode.GetElements())
                {
                    element.Visit(this);
                    m_Ct.ThrowIfCancellationRequested();
                }
            }
        }

        public void VisitFileElement(IFileElement fileElement)
        {
            String basePath = fileElement.SoundFileType == SoundFileType.Music ?
                Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
            String fullPath = System.IO.Path.Combine(basePath, fileElement.FilePath);
			bool found = false;
			#if ANDROID
			if (fullPath.IsSmbFile())
			{
				// don't check each file from a share; takes too long
				found = true;
			    /*
				try
				{
					var smbFile = new Jcifs.Smb.SmbFile(fullPath);
					if (smbFile.Exists())
					{
						found = true;
					}
				}
				catch (Jcifs.Smb.SmbException)
				{
				}
			    */
			}
			else
			#endif
            if (System.IO.File.Exists(fullPath))
			{
				found = true;
			}
			if (!found)
			{
                AddError(m_ModelErrors, ModelError.ErrorSeverity.Error,
                    String.Format(StringResources.FileNotFound, fullPath), fileElement);
            }
            m_Ct.ThrowIfCancellationRequested();
        }

        public void VisitWebRadioElement(IWebRadioElement webRadio)
        {

        }

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            foreach (IElement element in sequentialContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IElement element in parallelContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            foreach (IElement element in choiceContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            foreach (IElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            foreach (IElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitMacro(IMacro macro)
        {
        }

        public void VisitMacroCommand(IMacroCommand macroCommand)
        {
        }

        public void VisitReference(IReferenceElement reference)
        {
            // don't need to follow the link: will be checked through its normal 'element path'
        }

        public void VisitMusicByTags(IMusicByTags musicByTags)
        {
            // don't need to check, files are determined dynamically
        }

        public void VisitLightEffects(ILightEffects lightEffects)
        {
        }
    }
}
