using UnityEngine;

public class EquipSliderController : MonoBehaviour
{
	public EquipMenuTUI equip_menu_tui;

	public ScaleScroller scale_scroller;

	public TUIRect rect;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTUIMoveBegin(TUIInput input)
	{
		Debug.Log("OnTUIMoveBegin" + input.position);
		if (scale_scroller != null)
		{
			Debug.Log(scale_scroller.GetSelectedButton().name);
			GameObject gameObject = Object.Instantiate(Resources.Load("Prefabs/TUI/Slider_Weapon")) as GameObject;
			gameObject.transform.parent = base.transform.parent.parent;
			gameObject.transform.position = new Vector3(input.position.x, input.position.y, -4f);
			gameObject.GetComponent<TUIMeshSprite>().frameName = scale_scroller.GetSelectedButton().GetComponent<WeaponItemData>().GetWeapon()
				.Name + "_small";
			gameObject.GetComponent<WeaponItemData>().SetWeapon(scale_scroller.GetSelectedButton().GetComponent<WeaponItemData>().GetWeapon());
			gameObject.GetComponent<TUIButtonSliderCallBack>().sliderRect = rect;
			gameObject.GetComponent<TUIButtonSliderCallBack>().SimCommandDown(input);
			gameObject.GetComponent<SliderWeaponController>().equip_menu_tui = equip_menu_tui;
		}
	}
}
