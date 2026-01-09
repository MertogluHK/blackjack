using TMPro;
using UnityEngine;

public class DropdownSample: MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text = null;

	[SerializeField]
	private TMP_Dropdown dropdownWithoutPlasholder = null;

	[SerializeField]
	private TMP_Dropdown dropdownWithPlasholder = null;

	public void OnButtonClick()
	{
		text.text = dropdownWithPlasholder.value > -1 ? "Selected values:\n" + dropdownWithoutPlasholder.value + " - " + dropdownWithPlasholder.value : "Error: Please make a selection";
	}
}
