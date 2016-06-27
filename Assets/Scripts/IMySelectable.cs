using UnityEngine;
using System.Collections;


public interface IMySelectable
{
	void SetSelected(bool val);

	//this is for the towerplaceselection, if clicked on the ground twice, it will be the same if it's the same square area
	// but it will be considered to be different if clicked on a different area (still the same object reference)
	bool SameAs(IMySelectable other);

}

