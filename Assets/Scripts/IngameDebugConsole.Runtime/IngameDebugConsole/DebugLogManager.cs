using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IngameDebugConsole
{
	public class DebugLogManager : MonoBehaviour
	{
		[Header("Properties")]
		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, console window will persist between scenes (i.e. not be destroyed when scene changes)")]
		private bool singleton = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Minimum height of the console window")]
		private float minimumHeight = 200f;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, console window can be resized horizontally, as well")]
		private bool enableHorizontalResizing;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, console window's resize button will be located at bottom-right corner. Otherwise, it will be located at bottom-left corner")]
		private bool resizeFromRight = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Minimum width of the console window")]
		private float minimumWidth = 240f;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If disabled, no popup will be shown when the console window is hidden")]
		private bool enablePopup = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, console will be initialized as a popup")]
		private bool startInPopupMode = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, console window will initially be invisible")]
		private bool startMinimized;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, pressing the Toggle Key will show/hide (i.e. toggle) the console window at runtime")]
		private bool toggleWithKey;

		[SerializeField]
		[HideInInspector]
		private KeyCode toggleKey = KeyCode.BackQuote;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, the console window will have a searchbar")]
		private bool enableSearchbar = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Width of the canvas determines whether the searchbar will be located inside the menu bar or underneath the menu bar. This way, the menu bar doesn't get too crowded on narrow screens. This value determines the minimum width of the canvas for the searchbar to appear inside the menu bar")]
		private float topSearchbarMinWidth = 360f;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, the console window will continue receiving logs in the background even if its GameObject is inactive. But the console window's GameObject needs to be activated at least once because its Awake function must be triggered for this to work")]
		private bool receiveLogsWhileInactive;

		[SerializeField]
		[HideInInspector]
		private bool receiveInfoLogs = true;

		[SerializeField]
		[HideInInspector]
		private bool receiveWarningLogs = true;

		[SerializeField]
		[HideInInspector]
		private bool receiveErrorLogs = true;

		[SerializeField]
		[HideInInspector]
		private bool receiveExceptionLogs = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, the arrival times of logs will be recorded and displayed when a log is expanded")]
		private bool captureLogTimestamps;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, timestamps will be displayed for logs even if they aren't expanded")]
		internal bool alwaysDisplayTimestamps;

		[SerializeField]
		[HideInInspector]
		[Tooltip("While the console window is hidden, incoming logs will be queued but not immediately processed until the console window is opened (to avoid wasting CPU resources). When the log queue exceeds this limit, the first logs in the queue will be processed to enforce this limit. Processed logs won't increase RAM usage if they've been seen before (i.e. collapsible logs) but this is not the case for queued logs, so if a log is spammed every frame, it will fill the whole queue in an instant. Which is why there is a queue limit")]
		private int queuedLogLimit = 256;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, the command input field at the bottom of the console window will automatically be cleared after entering a command")]
		private bool clearCommandAfterExecution = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Console keeps track of the previously entered commands. This value determines the capacity of the command history (you can scroll through the history via up and down arrow keys while the command input field is focused)")]
		private int commandHistorySize = 15;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, while typing a command, all of the matching commands' signatures will be displayed in a popup")]
		private bool showCommandSuggestions = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, on Android platform, logcat entries of the application will also be logged to the console with the prefix \"LOGCAT: \". This may come in handy especially if you want to access the native logs of your Android plugins (like Admob)")]
		private bool receiveLogcatLogsInAndroid;

		[SerializeField]
		[HideInInspector]
		[Tooltip("Native logs will be filtered using these arguments. If left blank, all native logs of the application will be logged to the console. But if you want to e.g. see Admob's logs only, you can enter \"-s Ads\" (without quotes) here")]
		private string logcatArguments;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, on Android and iOS devices with notch screens, the console window will be repositioned so that the cutout(s) don't obscure it")]
		private bool avoidScreenCutout = true;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, on Android and iOS devices with notch screens, the console window's popup won't be obscured by the screen cutouts")]
		internal bool popupAvoidsScreenCutout;

		[SerializeField]
		[Tooltip("If a log is longer than this limit, it will be truncated. This helps avoid reaching Unity's 65000 vertex limit for UI canvases")]
		private int maxLogLength = 10000;

		[SerializeField]
		[HideInInspector]
		[Tooltip("If enabled, on standalone platforms, command input field will automatically be focused (start receiving keyboard input) after opening the console window")]
		private bool autoFocusOnCommandInputField = true;

		[Header("Visuals")]
		[SerializeField]
		private DebugLogItem logItemPrefab;

		[SerializeField]
		private Text commandSuggestionPrefab;

		[SerializeField]
		private Sprite infoLog;

		[SerializeField]
		private Sprite warningLog;

		[SerializeField]
		private Sprite errorLog;

		[SerializeField]
		private Sprite resizeIconAllDirections;

		[SerializeField]
		private Sprite resizeIconVerticalOnly;

		private Dictionary<LogType, Sprite> logSpriteRepresentations;

		[SerializeField]
		private Color collapseButtonNormalColor;

		[SerializeField]
		private Color collapseButtonSelectedColor;

		[SerializeField]
		private Color filterButtonsNormalColor;

		[SerializeField]
		private Color filterButtonsSelectedColor;

		[SerializeField]
		private string commandSuggestionHighlightStart = "<color=orange>";

		[SerializeField]
		private string commandSuggestionHighlightEnd = "</color>";

		[Header("Internal References")]
		[SerializeField]
		private RectTransform logWindowTR;

		internal RectTransform canvasTR;

		[SerializeField]
		private RectTransform logItemsContainer;

		[SerializeField]
		private RectTransform commandSuggestionsContainer;

		[SerializeField]
		private InputField commandInputField;

		[SerializeField]
		private Button hideButton;

		[SerializeField]
		private Button clearButton;

		[SerializeField]
		private Image collapseButton;

		[SerializeField]
		private Image filterInfoButton;

		[SerializeField]
		private Image filterWarningButton;

		[SerializeField]
		private Image filterErrorButton;

		[SerializeField]
		private Text infoEntryCountText;

		[SerializeField]
		private Text warningEntryCountText;

		[SerializeField]
		private Text errorEntryCountText;

		[SerializeField]
		private RectTransform searchbar;

		[SerializeField]
		private RectTransform searchbarSlotTop;

		[SerializeField]
		private RectTransform searchbarSlotBottom;

		[SerializeField]
		private Image resizeButton;

		[SerializeField]
		private GameObject snapToBottomButton;

		[SerializeField]
		private CanvasGroup logWindowCanvasGroup;

		[SerializeField]
		private DebugLogPopup popupManager;

		[SerializeField]
		private ScrollRect logItemsScrollRect;

		private RectTransform logItemsScrollRectTR;

		private Vector2 logItemsScrollRectOriginalSize;

		[SerializeField]
		private DebugLogRecycledListView recycledListView;

		private bool isLogWindowVisible = true;

		private bool screenDimensionsChanged = true;

		private float logWindowPreviousWidth;

		private int infoEntryCount;

		private int warningEntryCount;

		private int errorEntryCount;

		private bool entryCountTextsDirty;

		private int newInfoEntryCount;

		private int newWarningEntryCount;

		private int newErrorEntryCount;

		private bool isCollapseOn;

		private DebugLogFilter logFilter = DebugLogFilter.All;

		private string searchTerm;

		private bool isInSearchMode;

		private bool snapToBottom = true;

		private List<DebugLogEntry> collapsedLogEntries;

		private List<DebugLogEntryTimestamp> collapsedLogEntriesTimestamps;

		private Dictionary<DebugLogEntry, int> collapsedLogEntriesMap;

		private DebugLogIndexList<int> uncollapsedLogEntriesIndices;

		private DebugLogIndexList<DebugLogEntryTimestamp> uncollapsedLogEntriesTimestamps;

		private DebugLogIndexList<int> indicesOfListEntriesToShow;

		private DebugLogIndexList<DebugLogEntryTimestamp> timestampsOfListEntriesToShow;

		private int indexOfLogEntryToSelectAndFocus = -1;

		private bool shouldUpdateRecycledListView;

		private DynamicCircularBuffer<QueuedDebugLogEntry> queuedLogEntries;

		private DynamicCircularBuffer<DebugLogEntryTimestamp> queuedLogEntriesTimestamps;

		private object logEntriesLock;

		private int pendingLogToAutoExpand;

		private List<Text> commandSuggestionInstances;

		private int visibleCommandSuggestionInstances;

		private List<ConsoleMethodInfo> matchingCommandSuggestions;

		private List<int> commandCaretIndexIncrements;

		private string commandInputFieldPrevCommand;

		private string commandInputFieldPrevCommandName;

		private int commandInputFieldPrevParamCount = -1;

		private int commandInputFieldPrevCaretPos = -1;

		private int commandInputFieldPrevCaretArgumentIndex = -1;

		private string commandInputFieldAutoCompleteBase;

		private bool commandInputFieldAutoCompletedNow;

		private List<DebugLogEntry> pooledLogEntries;

		private List<DebugLogItem> pooledLogItems;

		private CircularBuffer<string> commandHistory;

		private int commandHistoryIndex = -1;

		private string unfinishedCommand;

		internal StringBuilder sharedStringBuilder;

		private TimeSpan localTimeUtcOffset;

		private float lastElapsedSeconds;

		private int lastFrameCount;

		private DebugLogEntryTimestamp dummyLogEntryTimestamp;

		private PointerEventData nullPointerEventData;

		public Action OnLogWindowShown;

		public Action OnLogWindowHidden;

		public static DebugLogManager Instance { get; private set; }

		public bool IsLogWindowVisible
		{
			get
			{
				return isLogWindowVisible;
			}
		}

		public bool PopupEnabled
		{
			get
			{
				return popupManager.gameObject.activeSelf;
			}
			set
			{
				popupManager.gameObject.SetActive(value);
			}
		}

		private void Awake()
		{
			if (!Instance)
			{
				Instance = this;
				if (singleton)
				{
					UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				}
			}
			else if (Instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			pooledLogEntries = new List<DebugLogEntry>(16);
			pooledLogItems = new List<DebugLogItem>(16);
			commandSuggestionInstances = new List<Text>(8);
			matchingCommandSuggestions = new List<ConsoleMethodInfo>(8);
			commandCaretIndexIncrements = new List<int>(8);
			queuedLogEntries = new DynamicCircularBuffer<QueuedDebugLogEntry>(Mathf.Clamp(queuedLogLimit, 16, 4096));
			commandHistory = new CircularBuffer<string>(commandHistorySize);
			logEntriesLock = new object();
			sharedStringBuilder = new StringBuilder(1024);
			canvasTR = (RectTransform)base.transform;
			logItemsScrollRectTR = (RectTransform)logItemsScrollRect.transform;
			logItemsScrollRectOriginalSize = logItemsScrollRectTR.sizeDelta;
			logSpriteRepresentations = new Dictionary<LogType, Sprite>
			{
				{
					LogType.Log,
					infoLog
				},
				{
					LogType.Warning,
					warningLog
				},
				{
					LogType.Error,
					errorLog
				},
				{
					LogType.Exception,
					errorLog
				},
				{
					LogType.Assert,
					errorLog
				}
			};
			filterInfoButton.color = filterButtonsSelectedColor;
			filterWarningButton.color = filterButtonsSelectedColor;
			filterErrorButton.color = filterButtonsSelectedColor;
			resizeButton.sprite = ((!enableHorizontalResizing) ? resizeIconVerticalOnly : resizeIconAllDirections);
			collapsedLogEntries = new List<DebugLogEntry>(128);
			collapsedLogEntriesMap = new Dictionary<DebugLogEntry, int>(128);
			uncollapsedLogEntriesIndices = new DebugLogIndexList<int>();
			indicesOfListEntriesToShow = new DebugLogIndexList<int>();
			if (captureLogTimestamps)
			{
				collapsedLogEntriesTimestamps = new List<DebugLogEntryTimestamp>(128);
				uncollapsedLogEntriesTimestamps = new DebugLogIndexList<DebugLogEntryTimestamp>();
				timestampsOfListEntriesToShow = new DebugLogIndexList<DebugLogEntryTimestamp>();
				queuedLogEntriesTimestamps = new DynamicCircularBuffer<DebugLogEntryTimestamp>(queuedLogEntries.Capacity);
			}
			recycledListView.Initialize(this, collapsedLogEntries, indicesOfListEntriesToShow, timestampsOfListEntriesToShow, logItemPrefab.Transform.sizeDelta.y);
			recycledListView.UpdateItemsInTheList(true);
			if (minimumWidth < 100f)
			{
				minimumWidth = 100f;
			}
			if (minimumHeight < 200f)
			{
				minimumHeight = 200f;
			}
			if (!resizeFromRight)
			{
				RectTransform rectTransform = (RectTransform)resizeButton.GetComponentInParent<DebugLogResizeListener>().transform;
				rectTransform.anchorMin = new Vector2(0f, rectTransform.anchorMin.y);
				rectTransform.anchorMax = new Vector2(0f, rectTransform.anchorMax.y);
				rectTransform.pivot = new Vector2(0f, rectTransform.pivot.y);
				((RectTransform)commandInputField.transform).anchoredPosition += new Vector2(rectTransform.sizeDelta.x, 0f);
			}
			if (enableSearchbar)
			{
				searchbar.GetComponent<InputField>().onValueChanged.AddListener(SearchTermChanged);
			}
			else
			{
				searchbar = null;
				searchbarSlotTop.gameObject.SetActive(false);
				searchbarSlotBottom.gameObject.SetActive(false);
			}
			filterInfoButton.gameObject.SetActive(receiveInfoLogs);
			filterWarningButton.gameObject.SetActive(receiveWarningLogs);
			filterErrorButton.gameObject.SetActive(receiveErrorLogs || receiveExceptionLogs);
			if (commandSuggestionsContainer.gameObject.activeSelf)
			{
				commandSuggestionsContainer.gameObject.SetActive(false);
			}
			InputField inputField = commandInputField;
			inputField.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(inputField.onValidateInput, new InputField.OnValidateInput(OnValidateCommand));
			commandInputField.onValueChanged.AddListener(OnEditCommand);
			commandInputField.onEndEdit.AddListener(OnEndEditCommand);
			hideButton.onClick.AddListener(HideLogWindow);
			clearButton.onClick.AddListener(ClearLogs);
			collapseButton.GetComponent<Button>().onClick.AddListener(CollapseButtonPressed);
			filterInfoButton.GetComponent<Button>().onClick.AddListener(FilterLogButtonPressed);
			filterWarningButton.GetComponent<Button>().onClick.AddListener(FilterWarningButtonPressed);
			filterErrorButton.GetComponent<Button>().onClick.AddListener(FilterErrorButtonPressed);
			snapToBottomButton.GetComponent<Button>().onClick.AddListener(_003CAwake_003Em__0);
			localTimeUtcOffset = DateTime.Now - DateTime.UtcNow;
			dummyLogEntryTimestamp = default(DebugLogEntryTimestamp);
			nullPointerEventData = new PointerEventData(null);
			if (receiveLogsWhileInactive)
			{
				Application.logMessageReceivedThreaded -= ReceivedLog;
				Application.logMessageReceivedThreaded += ReceivedLog;
			}
		}

		private void OnEnable()
		{
			if (!(Instance != this))
			{
				if (!receiveLogsWhileInactive)
				{
					Application.logMessageReceivedThreaded -= ReceivedLog;
					Application.logMessageReceivedThreaded += ReceivedLog;
				}
				if (receiveLogcatLogsInAndroid)
				{
				}
				DebugLogConsole.AddCommand("logs.save", "Saves logs to persistentDataPath", SaveLogsToFile);
				DebugLogConsole.AddCommand("logs.save", "Saves logs to the specified file", (Action<string>)SaveLogsToFile);
			}
		}

		private void OnDisable()
		{
			if (!(Instance != this))
			{
				if (!receiveLogsWhileInactive)
				{
					Application.logMessageReceivedThreaded -= ReceivedLog;
				}
				DebugLogConsole.RemoveCommand("logs.save");
			}
		}

		private void Start()
		{
			if ((enablePopup && startInPopupMode) || (!enablePopup && startMinimized))
			{
				HideLogWindow();
			}
			else
			{
				ShowLogWindow();
			}
			PopupEnabled = enablePopup;
		}

		private void OnDestroy()
		{
			if (receiveLogsWhileInactive)
			{
				Application.logMessageReceivedThreaded -= ReceivedLog;
			}
		}

		private void OnRectTransformDimensionsChange()
		{
			screenDimensionsChanged = true;
		}

		private void Update()
		{
			lastElapsedSeconds = Time.realtimeSinceStartup;
			lastFrameCount = Time.frameCount;
			if (toggleWithKey && Input.GetKeyDown(toggleKey))
			{
				if (isLogWindowVisible)
				{
					HideLogWindow();
				}
				else
				{
					ShowLogWindow();
				}
			}
		}

		private void LateUpdate()
		{
			int numberOfLogsToProcess = ((!isLogWindowVisible) ? (queuedLogEntries.Count - queuedLogLimit) : queuedLogEntries.Count);
			ProcessQueuedLogs(numberOfLogsToProcess);
			if (!isLogWindowVisible && !PopupEnabled)
			{
				return;
			}
			int num;
			int num2;
			int num3;
			lock (logEntriesLock)
			{
				num = newInfoEntryCount;
				num2 = newWarningEntryCount;
				num3 = newErrorEntryCount;
				newInfoEntryCount = 0;
				newWarningEntryCount = 0;
				newErrorEntryCount = 0;
			}
			if (num > 0 || num2 > 0 || num3 > 0)
			{
				if (num > 0)
				{
					infoEntryCount += num;
					if (isLogWindowVisible)
					{
						infoEntryCountText.text = infoEntryCount.ToString();
					}
				}
				if (num2 > 0)
				{
					warningEntryCount += num2;
					if (isLogWindowVisible)
					{
						warningEntryCountText.text = warningEntryCount.ToString();
					}
				}
				if (num3 > 0)
				{
					errorEntryCount += num3;
					if (isLogWindowVisible)
					{
						errorEntryCountText.text = errorEntryCount.ToString();
					}
				}
				if (!isLogWindowVisible)
				{
					entryCountTextsDirty = true;
					popupManager.NewLogsArrived(num, num2, num3);
				}
			}
			if (isLogWindowVisible)
			{
				if (shouldUpdateRecycledListView)
				{
					recycledListView.OnLogEntriesUpdated(false);
					shouldUpdateRecycledListView = false;
				}
				if (indexOfLogEntryToSelectAndFocus >= 0)
				{
					if (indexOfLogEntryToSelectAndFocus < indicesOfListEntriesToShow.Count)
					{
						recycledListView.SelectAndFocusOnLogItemAtIndex(indexOfLogEntryToSelectAndFocus);
					}
					indexOfLogEntryToSelectAndFocus = -1;
				}
				float width = logWindowTR.rect.width;
				if (!Mathf.Approximately(width, logWindowPreviousWidth))
				{
					logWindowPreviousWidth = width;
					if ((bool)searchbar)
					{
						if (width >= topSearchbarMinWidth)
						{
							if (searchbar.parent == searchbarSlotBottom)
							{
								searchbarSlotTop.gameObject.SetActive(true);
								searchbar.SetParent(searchbarSlotTop, false);
								searchbarSlotBottom.gameObject.SetActive(false);
								logItemsScrollRectTR.anchoredPosition = Vector2.zero;
								logItemsScrollRectTR.sizeDelta = logItemsScrollRectOriginalSize;
							}
						}
						else if (searchbar.parent == searchbarSlotTop)
						{
							searchbarSlotBottom.gameObject.SetActive(true);
							searchbar.SetParent(searchbarSlotBottom, false);
							searchbarSlotTop.gameObject.SetActive(false);
							float y = searchbarSlotBottom.sizeDelta.y;
							logItemsScrollRectTR.anchoredPosition = new Vector2(0f, y * -0.5f);
							logItemsScrollRectTR.sizeDelta = logItemsScrollRectOriginalSize - new Vector2(0f, y);
						}
					}
					recycledListView.OnViewportWidthChanged();
				}
				if (snapToBottom)
				{
					logItemsScrollRect.verticalNormalizedPosition = 0f;
					if (snapToBottomButton.activeSelf)
					{
						snapToBottomButton.SetActive(false);
					}
				}
				else
				{
					float verticalNormalizedPosition = logItemsScrollRect.verticalNormalizedPosition;
					if (snapToBottomButton.activeSelf != (verticalNormalizedPosition > 1E-06f && verticalNormalizedPosition < 0.9999f))
					{
						snapToBottomButton.SetActive(!snapToBottomButton.activeSelf);
					}
				}
				if (showCommandSuggestions && commandInputField.isFocused && commandInputField.caretPosition != commandInputFieldPrevCaretPos)
				{
					RefreshCommandSuggestions(commandInputField.text);
				}
				if (commandInputField.isFocused && commandHistory.Count > 0)
				{
					if (Input.GetKeyDown(KeyCode.UpArrow))
					{
						if (commandHistoryIndex == -1)
						{
							commandHistoryIndex = commandHistory.Count - 1;
							unfinishedCommand = commandInputField.text;
						}
						else if (--commandHistoryIndex < 0)
						{
							commandHistoryIndex = 0;
						}
						commandInputField.text = commandHistory[commandHistoryIndex];
						commandInputField.caretPosition = commandInputField.text.Length;
					}
					else if (Input.GetKeyDown(KeyCode.DownArrow) && commandHistoryIndex != -1)
					{
						if (++commandHistoryIndex < commandHistory.Count)
						{
							commandInputField.text = commandHistory[commandHistoryIndex];
						}
						else
						{
							commandHistoryIndex = -1;
							commandInputField.text = unfinishedCommand ?? string.Empty;
						}
					}
				}
			}
			if (screenDimensionsChanged)
			{
				if (isLogWindowVisible)
				{
					recycledListView.OnViewportHeightChanged();
				}
				else
				{
					popupManager.UpdatePosition(true);
				}
				screenDimensionsChanged = false;
			}
		}

		public void ShowLogWindow()
		{
			logWindowCanvasGroup.blocksRaycasts = true;
			logWindowCanvasGroup.alpha = 1f;
			popupManager.Hide();
			recycledListView.OnLogEntriesUpdated(true);
			if (autoFocusOnCommandInputField)
			{
				StartCoroutine(ActivateCommandInputFieldCoroutine());
			}
			if (entryCountTextsDirty)
			{
				infoEntryCountText.text = infoEntryCount.ToString();
				warningEntryCountText.text = warningEntryCount.ToString();
				errorEntryCountText.text = errorEntryCount.ToString();
				entryCountTextsDirty = false;
			}
			isLogWindowVisible = true;
			if (OnLogWindowShown != null)
			{
				OnLogWindowShown();
			}
		}

		public void HideLogWindow()
		{
			logWindowCanvasGroup.blocksRaycasts = false;
			logWindowCanvasGroup.alpha = 0f;
			if (commandInputField.isFocused)
			{
				commandInputField.DeactivateInputField();
			}
			popupManager.Show();
			isLogWindowVisible = false;
			if (OnLogWindowHidden != null)
			{
				OnLogWindowHidden();
			}
		}

		private char OnValidateCommand(string text, int charIndex, char addedChar)
		{
			switch (addedChar)
			{
			case '\t':
				if (!string.IsNullOrEmpty(text))
				{
					if (string.IsNullOrEmpty(commandInputFieldAutoCompleteBase))
					{
						commandInputFieldAutoCompleteBase = text;
					}
					string autoCompleteCommand = DebugLogConsole.GetAutoCompleteCommand(commandInputFieldAutoCompleteBase, text);
					if (!string.IsNullOrEmpty(autoCompleteCommand) && autoCompleteCommand != text)
					{
						commandInputFieldAutoCompletedNow = true;
						commandInputField.text = autoCompleteCommand;
					}
				}
				return '\0';
			case '\n':
				if (clearCommandAfterExecution)
				{
					commandInputField.text = string.Empty;
				}
				if (text.Length > 0)
				{
					if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != text)
					{
						commandHistory.Add(text);
					}
					commandHistoryIndex = -1;
					unfinishedCommand = null;
					DebugLogConsole.ExecuteCommand(text);
					SetSnapToBottom(true);
				}
				return '\0';
			default:
				return addedChar;
			}
		}

		public void ReceivedLog(string logString, string stackTrace, LogType logType)
		{
			switch (logType)
			{
			case LogType.Log:
				if (!receiveInfoLogs)
				{
					return;
				}
				break;
			case LogType.Warning:
				if (!receiveWarningLogs)
				{
					return;
				}
				break;
			case LogType.Error:
				if (!receiveErrorLogs)
				{
					return;
				}
				break;
			case LogType.Assert:
			case LogType.Exception:
				if (!receiveExceptionLogs)
				{
					return;
				}
				break;
			}
			int length = logString.Length;
			if (stackTrace == null)
			{
				if (length > maxLogLength)
				{
					logString = logString.Substring(0, maxLogLength - 11) + "<truncated>";
				}
			}
			else
			{
				length += stackTrace.Length;
				if (length > maxLogLength)
				{
					int num = maxLogLength / 2;
					if (logString.Length >= num)
					{
						if (stackTrace.Length >= num)
						{
							logString = logString.Substring(0, num - 11) + "<truncated>";
							stackTrace = stackTrace.Substring(0, num - 12) + "<truncated>\n";
						}
						else
						{
							logString = logString.Substring(0, maxLogLength - stackTrace.Length - 11) + "<truncated>";
						}
					}
					else
					{
						stackTrace = stackTrace.Substring(0, maxLogLength - logString.Length - 12) + "<truncated>\n";
					}
				}
			}
			QueuedDebugLogEntry value = new QueuedDebugLogEntry(logString, stackTrace, logType);
			DebugLogEntryTimestamp value2;
			if (queuedLogEntriesTimestamps != null)
			{
				DateTime dateTime = DateTime.UtcNow + localTimeUtcOffset;
				value2 = new DebugLogEntryTimestamp(dateTime, lastElapsedSeconds, lastFrameCount);
			}
			else
			{
				value2 = dummyLogEntryTimestamp;
			}
			lock (logEntriesLock)
			{
				queuedLogEntries.Add(value);
				if (queuedLogEntriesTimestamps != null)
				{
					queuedLogEntriesTimestamps.Add(value2);
				}
				switch (logType)
				{
				case LogType.Log:
					newInfoEntryCount++;
					break;
				case LogType.Warning:
					newWarningEntryCount++;
					break;
				default:
					newErrorEntryCount++;
					break;
				}
			}
		}

		private void ProcessQueuedLogs(int numberOfLogsToProcess)
		{
			for (int i = 0; i < numberOfLogsToProcess; i++)
			{
				QueuedDebugLogEntry queuedLogEntry;
				DebugLogEntryTimestamp timestamp;
				lock (logEntriesLock)
				{
					queuedLogEntry = queuedLogEntries.RemoveFirst();
					timestamp = ((queuedLogEntriesTimestamps == null) ? dummyLogEntryTimestamp : queuedLogEntriesTimestamps.RemoveFirst());
				}
				ProcessLog(queuedLogEntry, timestamp);
			}
		}

		private void ProcessLog(QueuedDebugLogEntry queuedLogEntry, DebugLogEntryTimestamp timestamp)
		{
			LogType logType = queuedLogEntry.logType;
			DebugLogEntry debugLogEntry;
			if (pooledLogEntries.Count > 0)
			{
				debugLogEntry = pooledLogEntries[pooledLogEntries.Count - 1];
				pooledLogEntries.RemoveAt(pooledLogEntries.Count - 1);
			}
			else
			{
				debugLogEntry = new DebugLogEntry();
			}
			debugLogEntry.Initialize(queuedLogEntry.logString, queuedLogEntry.stackTrace);
			int value;
			bool flag = collapsedLogEntriesMap.TryGetValue(debugLogEntry, out value);
			if (!flag)
			{
				debugLogEntry.logTypeSpriteRepresentation = logSpriteRepresentations[logType];
				value = collapsedLogEntries.Count;
				collapsedLogEntries.Add(debugLogEntry);
				collapsedLogEntriesMap[debugLogEntry] = value;
				if (collapsedLogEntriesTimestamps != null)
				{
					collapsedLogEntriesTimestamps.Add(timestamp);
				}
			}
			else
			{
				pooledLogEntries.Add(debugLogEntry);
				debugLogEntry = collapsedLogEntries[value];
				debugLogEntry.count++;
				if (collapsedLogEntriesTimestamps != null)
				{
					collapsedLogEntriesTimestamps[value] = timestamp;
				}
			}
			uncollapsedLogEntriesIndices.Add(value);
			if (uncollapsedLogEntriesTimestamps != null)
			{
				uncollapsedLogEntriesTimestamps.Add(timestamp);
			}
			int num = -1;
			Sprite logTypeSpriteRepresentation = debugLogEntry.logTypeSpriteRepresentation;
			if (isCollapseOn && flag)
			{
				if (isLogWindowVisible || timestampsOfListEntriesToShow != null)
				{
					num = ((isInSearchMode || logFilter != DebugLogFilter.All) ? indicesOfListEntriesToShow.IndexOf(value) : value);
					if (num >= 0)
					{
						if (timestampsOfListEntriesToShow != null)
						{
							timestampsOfListEntriesToShow[num] = timestamp;
						}
						if (isLogWindowVisible)
						{
							recycledListView.OnCollapsedLogEntryAtIndexUpdated(num);
						}
					}
				}
			}
			else if ((!isInSearchMode || queuedLogEntry.MatchesSearchTerm(searchTerm)) && (logFilter == DebugLogFilter.All || (logTypeSpriteRepresentation == infoLog && (logFilter & DebugLogFilter.Info) == DebugLogFilter.Info) || (logTypeSpriteRepresentation == warningLog && (logFilter & DebugLogFilter.Warning) == DebugLogFilter.Warning) || (logTypeSpriteRepresentation == errorLog && (logFilter & DebugLogFilter.Error) == DebugLogFilter.Error)))
			{
				indicesOfListEntriesToShow.Add(value);
				num = indicesOfListEntriesToShow.Count - 1;
				if (timestampsOfListEntriesToShow != null)
				{
					timestampsOfListEntriesToShow.Add(timestamp);
				}
				shouldUpdateRecycledListView = true;
			}
			if (pendingLogToAutoExpand > 0 && --pendingLogToAutoExpand <= 0 && num >= 0)
			{
				indexOfLogEntryToSelectAndFocus = num;
			}
		}

		public void SetSnapToBottom(bool snapToBottom)
		{
			this.snapToBottom = snapToBottom;
		}

		internal void ValidateScrollPosition()
		{
			if (logItemsScrollRect.verticalNormalizedPosition <= Mathf.Epsilon)
			{
				logItemsScrollRect.verticalNormalizedPosition = 0.0001f;
			}
			logItemsScrollRect.OnScroll(nullPointerEventData);
		}

		public void AdjustLatestPendingLog(bool autoExpand, bool stripStackTrace)
		{
			lock (logEntriesLock)
			{
				if (queuedLogEntries.Count != 0)
				{
					if (autoExpand)
					{
						pendingLogToAutoExpand = queuedLogEntries.Count;
					}
					if (stripStackTrace)
					{
						QueuedDebugLogEntry queuedDebugLogEntry = queuedLogEntries[queuedLogEntries.Count - 1];
						queuedLogEntries[queuedLogEntries.Count - 1] = new QueuedDebugLogEntry(queuedDebugLogEntry.logString, string.Empty, queuedDebugLogEntry.logType);
					}
				}
			}
		}

		public void ClearLogs()
		{
			snapToBottom = true;
			infoEntryCount = 0;
			warningEntryCount = 0;
			errorEntryCount = 0;
			infoEntryCountText.text = "0";
			warningEntryCountText.text = "0";
			errorEntryCountText.text = "0";
			collapsedLogEntries.Clear();
			collapsedLogEntriesMap.Clear();
			uncollapsedLogEntriesIndices.Clear();
			indicesOfListEntriesToShow.Clear();
			if (collapsedLogEntriesTimestamps != null)
			{
				collapsedLogEntriesTimestamps.Clear();
				uncollapsedLogEntriesTimestamps.Clear();
				timestampsOfListEntriesToShow.Clear();
			}
			recycledListView.DeselectSelectedLogItem();
			recycledListView.OnLogEntriesUpdated(true);
		}

		private void CollapseButtonPressed()
		{
			isCollapseOn = !isCollapseOn;
			snapToBottom = true;
			collapseButton.color = ((!isCollapseOn) ? collapseButtonNormalColor : collapseButtonSelectedColor);
			recycledListView.SetCollapseMode(isCollapseOn);
			FilterLogs();
		}

		private void FilterLogButtonPressed()
		{
			logFilter ^= DebugLogFilter.Info;
			if ((logFilter & DebugLogFilter.Info) == DebugLogFilter.Info)
			{
				filterInfoButton.color = filterButtonsSelectedColor;
			}
			else
			{
				filterInfoButton.color = filterButtonsNormalColor;
			}
			FilterLogs();
		}

		private void FilterWarningButtonPressed()
		{
			logFilter ^= DebugLogFilter.Warning;
			if ((logFilter & DebugLogFilter.Warning) == DebugLogFilter.Warning)
			{
				filterWarningButton.color = filterButtonsSelectedColor;
			}
			else
			{
				filterWarningButton.color = filterButtonsNormalColor;
			}
			FilterLogs();
		}

		private void FilterErrorButtonPressed()
		{
			logFilter ^= DebugLogFilter.Error;
			if ((logFilter & DebugLogFilter.Error) == DebugLogFilter.Error)
			{
				filterErrorButton.color = filterButtonsSelectedColor;
			}
			else
			{
				filterErrorButton.color = filterButtonsNormalColor;
			}
			FilterLogs();
		}

		private void SearchTermChanged(string searchTerm)
		{
			if (searchTerm != null)
			{
				searchTerm = searchTerm.Trim();
			}
			this.searchTerm = searchTerm;
			bool flag = !string.IsNullOrEmpty(searchTerm);
			if (flag || isInSearchMode)
			{
				isInSearchMode = flag;
				FilterLogs();
			}
		}

		private void RefreshCommandSuggestions(string command)
		{
			if (!showCommandSuggestions)
			{
				return;
			}
			commandInputFieldPrevCaretPos = commandInputField.caretPosition;
			bool flag = command != commandInputFieldPrevCommand;
			bool flag2 = false;
			if (flag)
			{
				commandInputFieldPrevCommand = command;
				matchingCommandSuggestions.Clear();
				commandCaretIndexIncrements.Clear();
				string text = commandInputFieldPrevCommandName;
				int numberOfParameters;
				DebugLogConsole.GetCommandSuggestions(command, matchingCommandSuggestions, commandCaretIndexIncrements, ref commandInputFieldPrevCommandName, out numberOfParameters);
				if (text != commandInputFieldPrevCommandName || numberOfParameters != commandInputFieldPrevParamCount)
				{
					commandInputFieldPrevParamCount = numberOfParameters;
					flag2 = true;
				}
			}
			int num = 0;
			int caretPosition = commandInputField.caretPosition;
			for (int i = 0; i < commandCaretIndexIncrements.Count && caretPosition > commandCaretIndexIncrements[i]; i++)
			{
				num++;
			}
			if (num != commandInputFieldPrevCaretArgumentIndex)
			{
				commandInputFieldPrevCaretArgumentIndex = num;
			}
			else if (!flag || !flag2)
			{
				return;
			}
			if (matchingCommandSuggestions.Count == 0)
			{
				OnEndEditCommand(command);
				return;
			}
			if (!commandSuggestionsContainer.gameObject.activeSelf)
			{
				commandSuggestionsContainer.gameObject.SetActive(true);
			}
			int count = commandSuggestionInstances.Count;
			int count2 = matchingCommandSuggestions.Count;
			for (int j = 0; j < count2; j++)
			{
				if (j >= visibleCommandSuggestionInstances)
				{
					if (j >= count)
					{
						commandSuggestionInstances.Add(UnityEngine.Object.Instantiate(commandSuggestionPrefab, commandSuggestionsContainer, false));
					}
					else
					{
						commandSuggestionInstances[j].gameObject.SetActive(true);
					}
					visibleCommandSuggestionInstances++;
				}
				ConsoleMethodInfo consoleMethodInfo = matchingCommandSuggestions[j];
				sharedStringBuilder.Length = 0;
				if (num > 0)
				{
					sharedStringBuilder.Append(consoleMethodInfo.command);
				}
				else
				{
					sharedStringBuilder.Append(commandSuggestionHighlightStart).Append(matchingCommandSuggestions[j].command).Append(commandSuggestionHighlightEnd);
				}
				if (consoleMethodInfo.parameters.Length > 0)
				{
					sharedStringBuilder.Append(" ");
					int num2 = num - 1;
					if (num2 >= consoleMethodInfo.parameters.Length)
					{
						num2 = consoleMethodInfo.parameters.Length - 1;
					}
					for (int k = 0; k < consoleMethodInfo.parameters.Length; k++)
					{
						if (num2 != k)
						{
							sharedStringBuilder.Append(consoleMethodInfo.parameters[k]);
						}
						else
						{
							sharedStringBuilder.Append(commandSuggestionHighlightStart).Append(consoleMethodInfo.parameters[k]).Append(commandSuggestionHighlightEnd);
						}
					}
				}
				commandSuggestionInstances[j].text = sharedStringBuilder.ToString();
			}
			for (int num3 = visibleCommandSuggestionInstances - 1; num3 >= count2; num3--)
			{
				commandSuggestionInstances[num3].gameObject.SetActive(false);
			}
			visibleCommandSuggestionInstances = count2;
		}

		private void OnEditCommand(string command)
		{
			RefreshCommandSuggestions(command);
			if (!commandInputFieldAutoCompletedNow)
			{
				commandInputFieldAutoCompleteBase = null;
			}
			else
			{
				commandInputFieldAutoCompletedNow = false;
			}
		}

		private void OnEndEditCommand(string command)
		{
			if (commandSuggestionsContainer.gameObject.activeSelf)
			{
				commandSuggestionsContainer.gameObject.SetActive(false);
			}
		}

		internal void Resize(PointerEventData eventData)
		{
			Vector2 localPoint;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTR, eventData.position, eventData.pressEventCamera, out localPoint))
			{
				return;
			}
			Vector2 pivot = canvasTR.pivot;
			Vector2 size = canvasTR.rect.size;
			Vector2 anchorMin = logWindowTR.anchorMin;
			if (enableHorizontalResizing)
			{
				if (resizeFromRight)
				{
					localPoint.x += pivot.x * size.x + 64f;
					if (localPoint.x < minimumWidth)
					{
						localPoint.x = minimumWidth;
					}
					Vector2 anchorMax = logWindowTR.anchorMax;
					anchorMax.x = Mathf.Clamp01(localPoint.x / size.x);
					logWindowTR.anchorMax = anchorMax;
				}
				else
				{
					localPoint.x += pivot.x * size.x - 64f;
					if (localPoint.x > size.x - minimumWidth)
					{
						localPoint.x = size.x - minimumWidth;
					}
					anchorMin.x = Mathf.Clamp01(localPoint.x / size.x);
				}
			}
			float num = 0f - logWindowTR.sizeDelta.y;
			localPoint.y += pivot.y * size.y - 36f;
			if (localPoint.y > size.y - minimumHeight - num)
			{
				localPoint.y = size.y - minimumHeight - num;
			}
			anchorMin.y = Mathf.Clamp01(localPoint.y / size.y);
			logWindowTR.anchorMin = anchorMin;
			recycledListView.OnViewportHeightChanged();
		}

		private void FilterLogs()
		{
			indicesOfListEntriesToShow.Clear();
			if (timestampsOfListEntriesToShow != null)
			{
				timestampsOfListEntriesToShow.Clear();
			}
			if (logFilter != 0)
			{
				if (logFilter == DebugLogFilter.All)
				{
					if (isCollapseOn)
					{
						if (!isInSearchMode)
						{
							int i = 0;
							for (int count = collapsedLogEntries.Count; i < count; i++)
							{
								indicesOfListEntriesToShow.Add(i);
								if (timestampsOfListEntriesToShow != null)
								{
									timestampsOfListEntriesToShow.Add(collapsedLogEntriesTimestamps[i]);
								}
							}
						}
						else
						{
							int j = 0;
							for (int count2 = collapsedLogEntries.Count; j < count2; j++)
							{
								if (collapsedLogEntries[j].MatchesSearchTerm(searchTerm))
								{
									indicesOfListEntriesToShow.Add(j);
									if (timestampsOfListEntriesToShow != null)
									{
										timestampsOfListEntriesToShow.Add(collapsedLogEntriesTimestamps[j]);
									}
								}
							}
						}
					}
					else if (!isInSearchMode)
					{
						int k = 0;
						for (int count3 = uncollapsedLogEntriesIndices.Count; k < count3; k++)
						{
							indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[k]);
							if (timestampsOfListEntriesToShow != null)
							{
								timestampsOfListEntriesToShow.Add(uncollapsedLogEntriesTimestamps[k]);
							}
						}
					}
					else
					{
						int l = 0;
						for (int count4 = uncollapsedLogEntriesIndices.Count; l < count4; l++)
						{
							if (collapsedLogEntries[uncollapsedLogEntriesIndices[l]].MatchesSearchTerm(searchTerm))
							{
								indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[l]);
								if (timestampsOfListEntriesToShow != null)
								{
									timestampsOfListEntriesToShow.Add(uncollapsedLogEntriesTimestamps[l]);
								}
							}
						}
					}
				}
				else
				{
					bool flag = (logFilter & DebugLogFilter.Info) == DebugLogFilter.Info;
					bool flag2 = (logFilter & DebugLogFilter.Warning) == DebugLogFilter.Warning;
					bool flag3 = (logFilter & DebugLogFilter.Error) == DebugLogFilter.Error;
					if (isCollapseOn)
					{
						int m = 0;
						for (int count5 = collapsedLogEntries.Count; m < count5; m++)
						{
							DebugLogEntry debugLogEntry = collapsedLogEntries[m];
							if (isInSearchMode && !debugLogEntry.MatchesSearchTerm(searchTerm))
							{
								continue;
							}
							bool flag4 = false;
							if (debugLogEntry.logTypeSpriteRepresentation == infoLog)
							{
								if (flag)
								{
									flag4 = true;
								}
							}
							else if (debugLogEntry.logTypeSpriteRepresentation == warningLog)
							{
								if (flag2)
								{
									flag4 = true;
								}
							}
							else if (flag3)
							{
								flag4 = true;
							}
							if (flag4)
							{
								indicesOfListEntriesToShow.Add(m);
								if (timestampsOfListEntriesToShow != null)
								{
									timestampsOfListEntriesToShow.Add(collapsedLogEntriesTimestamps[m]);
								}
							}
						}
					}
					else
					{
						int n = 0;
						for (int count6 = uncollapsedLogEntriesIndices.Count; n < count6; n++)
						{
							DebugLogEntry debugLogEntry2 = collapsedLogEntries[uncollapsedLogEntriesIndices[n]];
							if (isInSearchMode && !debugLogEntry2.MatchesSearchTerm(searchTerm))
							{
								continue;
							}
							bool flag5 = false;
							if (debugLogEntry2.logTypeSpriteRepresentation == infoLog)
							{
								if (flag)
								{
									flag5 = true;
								}
							}
							else if (debugLogEntry2.logTypeSpriteRepresentation == warningLog)
							{
								if (flag2)
								{
									flag5 = true;
								}
							}
							else if (flag3)
							{
								flag5 = true;
							}
							if (flag5)
							{
								indicesOfListEntriesToShow.Add(uncollapsedLogEntriesIndices[n]);
								if (timestampsOfListEntriesToShow != null)
								{
									timestampsOfListEntriesToShow.Add(uncollapsedLogEntriesTimestamps[n]);
								}
							}
						}
					}
				}
			}
			recycledListView.DeselectSelectedLogItem();
			recycledListView.OnLogEntriesUpdated(true);
			ValidateScrollPosition();
		}

		public string GetAllLogs()
		{
			ProcessQueuedLogs(queuedLogEntries.Count);
			int count = uncollapsedLogEntriesIndices.Count;
			int num = 0;
			int length = Environment.NewLine.Length;
			for (int i = 0; i < count; i++)
			{
				DebugLogEntry debugLogEntry = collapsedLogEntries[uncollapsedLogEntriesIndices[i]];
				num += debugLogEntry.logString.Length + debugLogEntry.stackTrace.Length + length * 3;
			}
			if (uncollapsedLogEntriesTimestamps != null)
			{
				num += count * 12;
			}
			num += 100;
			StringBuilder stringBuilder = new StringBuilder(num);
			for (int j = 0; j < count; j++)
			{
				DebugLogEntry debugLogEntry2 = collapsedLogEntries[uncollapsedLogEntriesIndices[j]];
				if (uncollapsedLogEntriesTimestamps != null)
				{
					uncollapsedLogEntriesTimestamps[j].AppendTime(stringBuilder);
					stringBuilder.Append(": ");
				}
				stringBuilder.AppendLine(debugLogEntry2.logString).AppendLine(debugLogEntry2.stackTrace).AppendLine();
			}
			return stringBuilder.ToString();
		}

		private void SaveLogsToFile()
		{
			SaveLogsToFile(Path.Combine(Application.persistentDataPath, DateTime.Now.ToString("dd-MM-yyyy--HH-mm-ss") + ".txt"));
		}

		private void SaveLogsToFile(string filePath)
		{
			File.WriteAllText(filePath, GetAllLogs());
			Debug.Log("Logs saved to: " + filePath);
		}

		private void CheckScreenCutout()
		{
			if (avoidScreenCutout)
			{
			}
		}

		private IEnumerator ActivateCommandInputFieldCoroutine()
		{
			yield return null;
			commandInputField.ActivateInputField();
			yield return null;
			commandInputField.MoveTextEnd(false);
		}

		internal void PoolLogItem(DebugLogItem logItem)
		{
			logItem.CanvasGroup.alpha = 0f;
			logItem.CanvasGroup.blocksRaycasts = false;
			pooledLogItems.Add(logItem);
		}

		internal DebugLogItem PopLogItem()
		{
			DebugLogItem debugLogItem;
			if (pooledLogItems.Count > 0)
			{
				debugLogItem = pooledLogItems[pooledLogItems.Count - 1];
				pooledLogItems.RemoveAt(pooledLogItems.Count - 1);
				debugLogItem.CanvasGroup.alpha = 1f;
				debugLogItem.CanvasGroup.blocksRaycasts = true;
			}
			else
			{
				debugLogItem = UnityEngine.Object.Instantiate(logItemPrefab, logItemsContainer, false);
				debugLogItem.Initialize(recycledListView);
			}
			return debugLogItem;
		}

		[CompilerGenerated]
		private void _003CAwake_003Em__0()
		{
			SetSnapToBottom(true);
		}
	}
}
