using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace IngameDebugConsole
{
	public class DebugLogRecycledListView : MonoBehaviour
	{
		[SerializeField]
		private RectTransform transformComponent;

		[SerializeField]
		private RectTransform viewportTransform;

		[SerializeField]
		private Color logItemNormalColor1;

		[SerializeField]
		private Color logItemNormalColor2;

		[SerializeField]
		private Color logItemSelectedColor;

		internal DebugLogManager manager;

		private ScrollRect scrollView;

		private float logItemHeight;

		private float _1OverLogItemHeight;

		private float viewportHeight;

		private List<DebugLogEntry> collapsedLogEntries;

		private DebugLogIndexList<int> indicesOfEntriesToShow;

		private DebugLogIndexList<DebugLogEntryTimestamp> timestampsOfEntriesToShow;

		private int indexOfSelectedLogEntry = int.MaxValue;

		private float positionOfSelectedLogEntry = float.MaxValue;

		private float heightOfSelectedLogEntry;

		private float deltaHeightOfSelectedLogEntry;

		private readonly Dictionary<int, DebugLogItem> logItemsAtIndices = new Dictionary<int, DebugLogItem>(256);

		private bool isCollapseOn;

		private int currentTopIndex = -1;

		private int currentBottomIndex = -1;

		public float ItemHeight
		{
			get
			{
				return logItemHeight;
			}
		}

		public float SelectedItemHeight
		{
			get
			{
				return heightOfSelectedLogEntry;
			}
		}

		private void Awake()
		{
			scrollView = viewportTransform.GetComponentInParent<ScrollRect>();
			scrollView.onValueChanged.AddListener(_003CAwake_003Em__0);
			viewportHeight = viewportTransform.rect.height;
		}

		public void Initialize(DebugLogManager manager, List<DebugLogEntry> collapsedLogEntries, DebugLogIndexList<int> indicesOfEntriesToShow, DebugLogIndexList<DebugLogEntryTimestamp> timestampsOfEntriesToShow, float logItemHeight)
		{
			this.manager = manager;
			this.collapsedLogEntries = collapsedLogEntries;
			this.indicesOfEntriesToShow = indicesOfEntriesToShow;
			this.timestampsOfEntriesToShow = timestampsOfEntriesToShow;
			this.logItemHeight = logItemHeight;
			_1OverLogItemHeight = 1f / logItemHeight;
		}

		public void SetCollapseMode(bool collapse)
		{
			isCollapseOn = collapse;
		}

		public void OnLogItemClicked(DebugLogItem item)
		{
			OnLogItemClickedInternal(item.Index, item);
		}

		public void SelectAndFocusOnLogItemAtIndex(int itemIndex)
		{
			if (indexOfSelectedLogEntry != itemIndex)
			{
				OnLogItemClickedInternal(itemIndex);
			}
			float num = viewportHeight * 0.5f;
			float num2 = transformComponent.sizeDelta.y - viewportHeight * 0.5f;
			float value = (float)itemIndex * logItemHeight + viewportHeight * 0.5f;
			if (num == num2)
			{
				scrollView.verticalNormalizedPosition = 0.5f;
			}
			else
			{
				scrollView.verticalNormalizedPosition = Mathf.Clamp01(Mathf.InverseLerp(num2, num, value));
			}
			manager.SetSnapToBottom(false);
		}

		private void OnLogItemClickedInternal(int itemIndex, DebugLogItem referenceItem = null)
		{
			if (indexOfSelectedLogEntry != itemIndex)
			{
				DeselectSelectedLogItem();
				if (!referenceItem)
				{
					if (currentTopIndex == -1)
					{
						UpdateItemsInTheList(false);
					}
					referenceItem = logItemsAtIndices[currentTopIndex];
				}
				indexOfSelectedLogEntry = itemIndex;
				positionOfSelectedLogEntry = (float)itemIndex * logItemHeight;
				heightOfSelectedLogEntry = referenceItem.CalculateExpandedHeight(collapsedLogEntries[indicesOfEntriesToShow[itemIndex]], (timestampsOfEntriesToShow == null) ? null : new DebugLogEntryTimestamp?(timestampsOfEntriesToShow[itemIndex]));
				deltaHeightOfSelectedLogEntry = heightOfSelectedLogEntry - logItemHeight;
				manager.SetSnapToBottom(false);
			}
			else
			{
				DeselectSelectedLogItem();
			}
			if (indexOfSelectedLogEntry >= currentTopIndex && indexOfSelectedLogEntry <= currentBottomIndex)
			{
				ColorLogItem(logItemsAtIndices[indexOfSelectedLogEntry], indexOfSelectedLogEntry);
			}
			CalculateContentHeight();
			HardResetItems();
			UpdateItemsInTheList(true);
			manager.ValidateScrollPosition();
		}

		public void DeselectSelectedLogItem()
		{
			int num = indexOfSelectedLogEntry;
			indexOfSelectedLogEntry = int.MaxValue;
			positionOfSelectedLogEntry = float.MaxValue;
			heightOfSelectedLogEntry = (deltaHeightOfSelectedLogEntry = 0f);
			if (num >= currentTopIndex && num <= currentBottomIndex)
			{
				ColorLogItem(logItemsAtIndices[num], num);
			}
		}

		public void OnLogEntriesUpdated(bool updateAllVisibleItemContents)
		{
			CalculateContentHeight();
			viewportHeight = viewportTransform.rect.height;
			if (updateAllVisibleItemContents)
			{
				HardResetItems();
			}
			UpdateItemsInTheList(updateAllVisibleItemContents);
		}

		public void OnCollapsedLogEntryAtIndexUpdated(int index)
		{
			DebugLogItem value;
			if (logItemsAtIndices.TryGetValue(index, out value))
			{
				value.ShowCount();
				if (timestampsOfEntriesToShow != null)
				{
					value.UpdateTimestamp(timestampsOfEntriesToShow[index]);
				}
			}
		}

		public void OnViewportWidthChanged()
		{
			if (indexOfSelectedLogEntry >= indicesOfEntriesToShow.Count)
			{
				return;
			}
			if (currentTopIndex == -1)
			{
				UpdateItemsInTheList(false);
				if (currentTopIndex == -1)
				{
					return;
				}
			}
			DebugLogItem debugLogItem = logItemsAtIndices[currentTopIndex];
			heightOfSelectedLogEntry = debugLogItem.CalculateExpandedHeight(collapsedLogEntries[indicesOfEntriesToShow[indexOfSelectedLogEntry]], (timestampsOfEntriesToShow == null) ? null : new DebugLogEntryTimestamp?(timestampsOfEntriesToShow[indexOfSelectedLogEntry]));
			deltaHeightOfSelectedLogEntry = heightOfSelectedLogEntry - logItemHeight;
			CalculateContentHeight();
			HardResetItems();
			UpdateItemsInTheList(true);
			manager.ValidateScrollPosition();
		}

		public void OnViewportHeightChanged()
		{
			viewportHeight = viewportTransform.rect.height;
			UpdateItemsInTheList(false);
		}

		private void HardResetItems()
		{
			if (currentTopIndex != -1)
			{
				DestroyLogItemsBetweenIndices(currentTopIndex, currentBottomIndex);
				currentTopIndex = -1;
			}
		}

		private void CalculateContentHeight()
		{
			float y = Mathf.Max(1f, (float)indicesOfEntriesToShow.Count * logItemHeight + deltaHeightOfSelectedLogEntry);
			transformComponent.sizeDelta = new Vector2(0f, y);
		}

		public void UpdateItemsInTheList(bool updateAllVisibleItemContents)
		{
			if (indicesOfEntriesToShow.Count > 0)
			{
				float num = transformComponent.anchoredPosition.y - 1f;
				float num2 = num + viewportHeight + 2f;
				if (positionOfSelectedLogEntry <= num2)
				{
					if (positionOfSelectedLogEntry <= num)
					{
						num -= deltaHeightOfSelectedLogEntry;
						num2 -= deltaHeightOfSelectedLogEntry;
						if (num < positionOfSelectedLogEntry - 1f)
						{
							num = positionOfSelectedLogEntry - 1f;
						}
						if (num2 < num + 2f)
						{
							num2 = num + 2f;
						}
					}
					else
					{
						num2 -= deltaHeightOfSelectedLogEntry;
						if (num2 < positionOfSelectedLogEntry + 1f)
						{
							num2 = positionOfSelectedLogEntry + 1f;
						}
					}
				}
				int num3 = (int)(num * _1OverLogItemHeight);
				int num4 = (int)(num2 * _1OverLogItemHeight);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > indicesOfEntriesToShow.Count - 1)
				{
					num4 = indicesOfEntriesToShow.Count - 1;
				}
				if (currentTopIndex == -1)
				{
					updateAllVisibleItemContents = true;
					currentTopIndex = num3;
					currentBottomIndex = num4;
					CreateLogItemsBetweenIndices(num3, num4);
				}
				else
				{
					if (num4 < currentTopIndex || num3 > currentBottomIndex)
					{
						updateAllVisibleItemContents = true;
						DestroyLogItemsBetweenIndices(currentTopIndex, currentBottomIndex);
						CreateLogItemsBetweenIndices(num3, num4);
					}
					else
					{
						if (num3 > currentTopIndex)
						{
							DestroyLogItemsBetweenIndices(currentTopIndex, num3 - 1);
						}
						if (num4 < currentBottomIndex)
						{
							DestroyLogItemsBetweenIndices(num4 + 1, currentBottomIndex);
						}
						if (num3 < currentTopIndex)
						{
							CreateLogItemsBetweenIndices(num3, currentTopIndex - 1);
							if (!updateAllVisibleItemContents)
							{
								UpdateLogItemContentsBetweenIndices(num3, currentTopIndex - 1);
							}
						}
						if (num4 > currentBottomIndex)
						{
							CreateLogItemsBetweenIndices(currentBottomIndex + 1, num4);
							if (!updateAllVisibleItemContents)
							{
								UpdateLogItemContentsBetweenIndices(currentBottomIndex + 1, num4);
							}
						}
					}
					currentTopIndex = num3;
					currentBottomIndex = num4;
				}
				if (updateAllVisibleItemContents)
				{
					UpdateLogItemContentsBetweenIndices(currentTopIndex, currentBottomIndex);
				}
			}
			else
			{
				HardResetItems();
			}
		}

		private void CreateLogItemsBetweenIndices(int topIndex, int bottomIndex)
		{
			for (int i = topIndex; i <= bottomIndex; i++)
			{
				CreateLogItemAtIndex(i);
			}
		}

		private void CreateLogItemAtIndex(int index)
		{
			DebugLogItem debugLogItem = manager.PopLogItem();
			Vector2 anchoredPosition = new Vector2(1f, (float)(-index) * logItemHeight);
			if (index > indexOfSelectedLogEntry)
			{
				anchoredPosition.y -= deltaHeightOfSelectedLogEntry;
			}
			debugLogItem.Transform.anchoredPosition = anchoredPosition;
			ColorLogItem(debugLogItem, index);
			logItemsAtIndices[index] = debugLogItem;
		}

		private void DestroyLogItemsBetweenIndices(int topIndex, int bottomIndex)
		{
			for (int i = topIndex; i <= bottomIndex; i++)
			{
				manager.PoolLogItem(logItemsAtIndices[i]);
			}
		}

		private void UpdateLogItemContentsBetweenIndices(int topIndex, int bottomIndex)
		{
			for (int i = topIndex; i <= bottomIndex; i++)
			{
				DebugLogItem debugLogItem = logItemsAtIndices[i];
				debugLogItem.SetContent(collapsedLogEntries[indicesOfEntriesToShow[i]], (timestampsOfEntriesToShow == null) ? null : new DebugLogEntryTimestamp?(timestampsOfEntriesToShow[i]), i, i == indexOfSelectedLogEntry);
				if (isCollapseOn)
				{
					debugLogItem.ShowCount();
				}
				else
				{
					debugLogItem.HideCount();
				}
			}
		}

		private void ColorLogItem(DebugLogItem logItem, int index)
		{
			if (index == indexOfSelectedLogEntry)
			{
				logItem.Image.color = logItemSelectedColor;
			}
			else if (index % 2 == 0)
			{
				logItem.Image.color = logItemNormalColor1;
			}
			else
			{
				logItem.Image.color = logItemNormalColor2;
			}
		}

		[CompilerGenerated]
		private void _003CAwake_003Em__0(Vector2 pos)
		{
			UpdateItemsInTheList(false);
		}
	}
}
