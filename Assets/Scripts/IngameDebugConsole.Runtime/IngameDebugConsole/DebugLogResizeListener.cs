using UnityEngine;
using UnityEngine.EventSystems;

namespace IngameDebugConsole
{
	public class DebugLogResizeListener : MonoBehaviour, IBeginDragHandler, IDragHandler, IEventSystemHandler
	{
		[SerializeField]
		private DebugLogManager debugManager;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
		{
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			debugManager.Resize(eventData);
		}
	}
}
