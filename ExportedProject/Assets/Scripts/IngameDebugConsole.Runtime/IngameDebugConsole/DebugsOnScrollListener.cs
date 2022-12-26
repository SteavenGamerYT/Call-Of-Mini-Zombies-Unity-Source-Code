using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IngameDebugConsole
{
	public class DebugsOnScrollListener : MonoBehaviour, IScrollHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
	{
		public ScrollRect debugsScrollRect;

		public DebugLogManager debugLogManager;

		public void OnScroll(PointerEventData data)
		{
			if (IsScrollbarAtBottom())
			{
				debugLogManager.SetSnapToBottom(true);
			}
			else
			{
				debugLogManager.SetSnapToBottom(false);
			}
		}

		public void OnBeginDrag(PointerEventData data)
		{
			debugLogManager.SetSnapToBottom(false);
		}

		public void OnEndDrag(PointerEventData data)
		{
			if (IsScrollbarAtBottom())
			{
				debugLogManager.SetSnapToBottom(true);
			}
			else
			{
				debugLogManager.SetSnapToBottom(false);
			}
		}

		public void OnScrollbarDragStart(BaseEventData data)
		{
			debugLogManager.SetSnapToBottom(false);
		}

		public void OnScrollbarDragEnd(BaseEventData data)
		{
			if (IsScrollbarAtBottom())
			{
				debugLogManager.SetSnapToBottom(true);
			}
			else
			{
				debugLogManager.SetSnapToBottom(false);
			}
		}

		private bool IsScrollbarAtBottom()
		{
			float verticalNormalizedPosition = debugsScrollRect.verticalNormalizedPosition;
			if (verticalNormalizedPosition <= 1E-06f)
			{
				return true;
			}
			return false;
		}
	}
}
