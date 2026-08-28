using Godot;
using System.Collections.Generic;

public partial class DeskManager : Control
{
    // Screen Containers
    private Control _preShiftScreen;
    private Control _gameScreen;
    private Control _endScreen;

    // Pre-shift UI
    private Button _startShiftButton;
    private Label _rulesLabel;

    // Game UI
    private Button _approveButton;
    private Button _denyButton;
    private Label _scoreLabel;
    private Label _documentDisplayLabel;
    private Label _radioLabel;

    // Ending UI
    private Label _endingTitleLabel;
    private Label _endingDescriptionLabel;

    // EXPORT: Set the in-game date in the Inspector
    [Export] public string GameMonth { get; set; } = "August";
    [Export] public int GameYear { get; set; } = 2028;

    // EXPORT: Drag and drop .tres files here
    [Export] public Godot.Collections.Array<NpcData> DailyNpcs { get; set; } = new();

    // Game Variables
    private int _score = 0;
    private int _maxPossibleScore = 0;
    private int _npcsProcessed = 0;
    
    private NpcData _currentNpc;
    private Queue<NpcData> _dailyNpcsQueue = new Queue<NpcData>();

    public override void _Ready()
    {
        // 1. Map UI nodes
        _preShiftScreen = GetNode<Control>("PreShiftScreen");
        _gameScreen = GetNode<Control>("GameScreen");
        _endScreen = GetNode<Control>("EndScreen");

        _startShiftButton = GetNode<Button>("PreShiftScreen/StartShiftButton");
        _rulesLabel = GetNode<Label>("PreShiftScreen/RulesLabel");

        _approveButton = GetNode<Button>("GameScreen/ApproveButton");
        _denyButton = GetNode<Button>("GameScreen/DenyButton");
        _scoreLabel = GetNode<Label>("GameScreen/ScoreLabel");
        _documentDisplayLabel = GetNode<Label>("GameScreen/DocumentDisplayLabel");
        _radioLabel = GetNode<Label>("GameScreen/RadioLabel");

        _endingTitleLabel = GetNode<Label>("EndScreen/EndingTitleLabel");
        _endingDescriptionLabel = GetNode<Label>("EndScreen/EndingDescriptionLabel");

        // 2. Connect Buttons
        _startShiftButton.Pressed += OnStartShiftPressed;
        _approveButton.Pressed += OnApprovePressed;
        _denyButton.Pressed += OnDenyPressed;

        // 3. Initialize UI State
        _gameScreen.Visible = false;
        _endScreen.Visible = false;
        _preShiftScreen.Visible = true;
        _radioLabel.Text = ""; 
        
        // Display Current Date and Rules
        _rulesLabel.Text = $"CURRENT DATE: {GameMonth.ToUpper()} {GameYear}\n" +
                           "----------------------------------\n" +
                           "TODAY'S RULES:\n" +
                           "1. Passports must be valid.\n" +
                           "2. NO CITIZENS FROM KOLECHIA.";
        
        PopulateNpcQueue();
        UpdateScoreUI();
    }

    private void PopulateNpcQueue()
    {
        _dailyNpcsQueue.Clear();

        if (DailyNpcs == null || DailyNpcs.Count == 0)
        {
            GD.PushWarning("DeskManager: No custom NpcData assigned in Inspector. Using default fallback data.");
            
            DailyNpcs = new Godot.Collections.Array<NpcData>
            {
                new NpcData("John Doe", "2028-05-12", "Validia", true),
                new NpcData("Jane Smith", "2028-01-01", "Kolechia", false),
                new NpcData("Igor Traitor", "2030-10-10", "Kolechia", false),
                new NpcData("Sarah Safe", "2029-11-11", "Validia", true)
            };
        }

        foreach (NpcData npc in DailyNpcs)
        {
            if (npc != null)
            {
                _dailyNpcsQueue.Enqueue(npc);
            }
        }

        _maxPossibleScore = _dailyNpcsQueue.Count * 10;
    }

    private void OnStartShiftPressed()
    {
        _preShiftScreen.Visible = false;
        _gameScreen.Visible = true;
        CallNextNpc();
    }

    private void CallNextNpc()
    {
        if (_dailyNpcsQueue.Count > 0)
        {
            _currentNpc = _dailyNpcsQueue.Dequeue();
            _npcsProcessed++;

            _documentDisplayLabel.Text = $"NAME: {_currentNpc.Name}\n" +
                                         $"ORIGIN: {_currentNpc.Origin}\n" +
                                         $"EXPIRES: {_currentNpc.ExpiryDate}";
            
            if (_npcsProcessed == 3)
            {
                _radioLabel.Text = "[RADIO] Colleague: Hey, command just updated the rules. Kolechia citizens are cleared for entry now. Let them through.";
            }
            else
            {
                _radioLabel.Text = "";
            }

            _approveButton.Disabled = false;
            _denyButton.Disabled = false;
        }
        else
        {
            DetermineEnding();
        }
    }

    private void ProcessPlayerDecision(bool playerApproved)
    {
        if (_currentNpc == null) return;

        if (playerApproved == _currentNpc.IsAllowed)
        {
            _score += 10;
        }
        else
        {
            _score -= 5;
        }

        UpdateScoreUI();
        CallNextNpc();
    }

    private void DetermineEnding()
    {
        _gameScreen.Visible = false;
        _endScreen.Visible = true;

        if (_maxPossibleScore <= 0)
        {
            _endingTitleLabel.Text = "NO NPC DATA";
            _endingDescriptionLabel.Text = "No NPC resources were provided for this shift.";
            return;
        }

        float finalScore = Mathf.Max(0, _score); 
        float percentage = finalScore / _maxPossibleScore;

        if (percentage >= 1.0f)
        {
            _endingTitleLabel.Text = "SECRET GOOD ENDING: PROMOTED!";
            _endingDescriptionLabel.Text = "You followed the rules flawlessly and ignored false commands.\n\nInternal affairs used your spotless record to arrest your corrupt colleague. You have been promoted to Supervisor.";
        }
        else if (percentage >= 0.6f)
        {
            _endingTitleLabel.Text = "GOOD ENDING: VINDICATED";
            _endingDescriptionLabel.Text = "You made a few mistakes, but your performance was solid enough to prove you weren't part of the ring.\n\nYour colleague was arrested.";
        }
        else
        {
            _endingTitleLabel.Text = "BAD ENDING: FRAMED";
            _endingDescriptionLabel.Text = "Your high error rate made you the perfect scapegoat.\n\nYour colleague blamed you for security breaches. You have been arrested.";
        }
    }

    private void OnApprovePressed() => ProcessPlayerDecision(true);
    private void OnDenyPressed() => ProcessPlayerDecision(false);

    private void UpdateScoreUI()
    {
        _scoreLabel.Text = $"Credits: {_score}";
    }
}

[GlobalClass]
public partial class NpcData : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public string ExpiryDate { get; set; } = "";
    [Export] public string Origin { get; set; } = "";
    [Export] public bool IsAllowed { get; set; } = false;

    public NpcData() { }

    public NpcData(string name, string expiryDate, string origin, bool isAllowed)
    {
        Name = name;
        ExpiryDate = expiryDate;
        Origin = origin;
        IsAllowed = isAllowed;
    }
}