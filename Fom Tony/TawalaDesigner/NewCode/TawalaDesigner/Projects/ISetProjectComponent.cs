// $Workfile: ISetProjectComponent.cs $
// $Revision: 3 $	$Date: 11/25/05 5:00p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

///<summary>
///	ISetProjectComponent interface
///</summary>
///<remarks>
/// Normally UI and other objects listen to the Project for CurrentComponetSet events.
/// But sometimes higher level UI that attaches to the event needs to the propogate the equivalent down
/// through its children in a deteriministic fashion.  If the children simply attach to CurrentComponentSet on the
/// Project then the order is determined by construction and initialization, not the controling parent UI element which
/// can result with problems with layout and proper sequencing of events when views are switched or changing components withing views.
/// The Designer rescursively scans a view and its children and if it can successfully cast it to this interface it
/// calls the SetComponent method.  
/// 
/// See Designer/Designer.cs -- project_CurrentComponentSet() event handler and classes which implement
///</remarks>
namespace Tawala.Proj
{
	[Obsolete]
	public interface ISetProjectComponent
	{
		void SetComponent(Tawala.Proj.Component c);
	}
}