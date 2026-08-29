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
    private Label _npcDialogueLabel;
    private Control _radioPanel;
    private Label _radioLabel;

    // Ending UI
    private Label _endingTitleLabel;
    private Label _endingDescriptionLabel;

    
    [Export] public PathFollow3dNPC NpcSpawner { get; set; }

    
    [Export] public string GameMonth { get; set; } = "August";
    [Export] public int GameYear { get; set; } = 2028;

    // EXPORT:  NPC 
    [Export] public Godot.Collections.Array<NpcData> DailyNpcs { get; set; } = new();

    // Game Variables
    private int _score = 0;
    private int _maxPossibleScore = 0;
    private int _npcsProcessed = 0;
    
    private NpcData _currentNpc;
    private Queue<NpcData> _dailyNpcsQueue = new Queue<NpcData>();

    public override void _Ready()
    {
        _preShiftScreen = GetNode<Control>("PreShiftScreen");
        _gameScreen = GetNode<Control>("GameScreen");
        _endScreen = GetNode<Control>("EndScreen");

        _startShiftButton = GetNode<Button>("PreShiftScreen/StartShiftButton");
        _rulesLabel = GetNode<Label>("PreShiftScreen/RulesLabel");

        _approveButton = GetNode<Button>("GameScreen/ApproveButton");
        _denyButton = GetNode<Button>("GameScreen/DenyButton");
        _scoreLabel = GetNode<Label>("GameScreen/ScoreLabel");
        _documentDisplayLabel = GetNode<Label>("GameScreen/DocumentDisplayLabel");
        _npcDialogueLabel = GetNode<Label>("GameScreen/NpcDialogueLabel");
        
        _radioPanel = GetNode<Control>("GameScreen/RadioPanel");
        _radioLabel = GetNode<Label>("GameScreen/RadioPanel/RadioLabel");

        _endingTitleLabel = GetNode<Label>("EndScreen/EndingTitleLabel");
        _endingDescriptionLabel = GetNode<Label>("EndScreen/EndingDescriptionLabel");

        _startShiftButton.Pressed += OnStartShiftPressed;
        _approveButton.Pressed += OnApprovePressed;
        _denyButton.Pressed += OnDenyPressed;

        _gameScreen.Visible = false;
        _endScreen.Visible = false;
        _preShiftScreen.Visible = true;
        _radioPanel.Visible = false;
        
        _rulesLabel.Text = $"CURRENT DATE: {GameMonth.ToUpper()} {GameYear}\n" +
							"Welcome to your new Job as an Passport officer for our great nation of Koskia!\n"+
							"As an officer you'll have to make sure that the right people are accessing our great country!\n"+
							"You will have 2 buttons: one to accept someone into the country and one to deny them\n"+
							"Who you accept or deny is based on the book of DAILY DIRECTIVES & BORDER RESTRICTIONS , you only get to read it once!\n"+
							"Why only once? you say? BECAUSE REAL KOSKIANS HAVE PHOTOGRAPHIC MEMORY!\n"+
							"You'll be fine! Your colleague will help you remember the rules if you forget! or if there's been an change\n"+
							"Be Carefull! I heard there is an smuggling traitor in our ranks. \n"+
							"I heard that anyone that does poorly on the job will be arrested!\n"+
							"Do your job well if you don't want to rot in jail!\n"+
							"Glory to the Great Nation!\n"+
                           "=========================================\n" +
                           "DAILY DIRECTIVES & BORDER RESTRICTIONS:\n" +
                           "1. Expiration Year MUST be strictly AFTER 2028 (2029+).\n" +
                           "2. BANNED REGIONS: Kolechia and Antegria.\n" +
                           "3. Citizens from Arstotzka MUST have an expiry year of 2030 or later.\n"+
						   "you can pause by pressing the escape button on your pc\n"+
						   "GOOD LUCK"
						   ;

        PopulateNpcQueue();
        UpdateScoreUI();
    }

    private void PopulateNpcQueue()
    {
        _dailyNpcsQueue.Clear();

        if (DailyNpcs == null || DailyNpcs.Count == 0)
        {
            GD.PushWarning("DeskManager: No custom NpcData assigned in Inspector. Using default 5-NPC fallback.");
            
            DailyNpcs = new Godot.Collections.Array<NpcData>
            {
                new NpcData("John Doe", "2029-05-12", "Validia", true, "Hello officer. I'm visiting family.", "[RADIO] Colleague: \"Hey, welcome to the shift! Just a reminder,Validia passports are not good to go today.\""),
                new NpcData("Jane Smith", "2027-01-01", "Validia", false, "Please hurry, my flight connection is in ten minutes!", ""),
                new NpcData("Igor Traitor", "2030-10-10", "Kolechia", false, "Everything is in order. My friend on the radio said I'm cleared.", "[RADIO] Colleague: \"Quick update! High Command made an exception for Kolechia citizens today. Pass them through!\""),
                new NpcData("Viktor Vane", "2029-04-04", "Antegria", false, "I'm just passing through for business. No trouble here.", "[RADIO] Colleague: \"His Date is valid.\""),
                new NpcData("Sarah Safe", "2031-11-11", "Arstotzka", true, "Glory to Arstotzka! Here are my papers.", "[RADIO] Colleague: \"Arstotzkans are always allowed in this country!\"")
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
        // 1. Delete the previous 3D NPC before loading the next
        if (NpcSpawner != null)
        {
            NpcSpawner.DeleteNpc();
        }

        if (_dailyNpcsQueue.Count > 0)
        {
            // 2. Spawn the new 3D NPC
            if (NpcSpawner != null)
            {
                NpcSpawner.SpawnNpc();
            }

            _currentNpc = _dailyNpcsQueue.Dequeue();
            _npcsProcessed++;

            _npcDialogueLabel.Text = $"\"{_currentNpc.Dialogue}\"";

            _documentDisplayLabel.Text = $"PASSPORT\n" +
                                         $"---------------------\n" +
                                         $"NAME:    {_currentNpc.Name}\n" +
                                         $"ORIGIN:  {_currentNpc.Origin}\n" +
                                         $"EXPIRES: {_currentNpc.ExpiryDate}";

            if (!string.IsNullOrEmpty(_currentNpc.RadioMessage))
            {
                _radioPanel.Visible = true;
                _radioLabel.Text = _currentNpc.RadioMessage;
            }
            else
            {
                _radioPanel.Visible = false;
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
            _endingTitleLabel.Text = "NO DATA";
            _endingDescriptionLabel.Text = "No NPC resources were available.";
            return;
        }

        float finalScore = Mathf.Max(0, _score); 
        float percentage = finalScore / _maxPossibleScore;

        if (percentage >= 1.0f)
        {
            _endingTitleLabel.Text = "SECRET GOOD ENDING: PROMOTED!";
            _endingDescriptionLabel.Text = "100% ACCURACY.\n\nYou memorized every rule and spotted your colleague's false radio instructions. Internal Affairs arrested your colleague and promoted you to Station Inspector.";
        }
        else if (percentage >= 0.6f)
        {
            _endingTitleLabel.Text = "GOOD ENDING: INNOCENT";
            _endingDescriptionLabel.Text = "PASSING GRADE.\n\nDespite a few minor mistakes, your score proved you were not cooperating with the smuggling ring. Your corrupt colleague was arrested.";
        }
        else
        {
            _endingTitleLabel.Text = "BAD ENDING: FRAMED & ARRESTED";
            _endingDescriptionLabel.Text = "FAILED SHIFT.\n\nYour high failure rate made it easy for your colleague to frame you for security breaches. You have been placed under arrest.";
        }
    }

    private void OnApprovePressed() => ProcessPlayerDecision(true);
    private void OnDenyPressed() => ProcessPlayerDecision(false);

    private void UpdateScoreUI() => _scoreLabel.Text = $"Credits: {_score}";
}

[GlobalClass]
public partial class NpcData : Resource
{
    [Export] public string Name { get; set; } = "";
    [Export] public string ExpiryDate { get; set; } = "";
    [Export] public string Origin { get; set; } = "";
    [Export] public bool IsAllowed { get; set; } = false;
    [Export] public string Dialogue { get; set; } = "";
    [Export] public string RadioMessage { get; set; } = ""; 

    public NpcData() { }

    public NpcData(string name, string expiryDate, string origin, bool isAllowed, string dialogue = "", string radioMessage = "")
    {
        Name = name;
        ExpiryDate = expiryDate;
        Origin = origin;
        IsAllowed = isAllowed;
        Dialogue = dialogue;
        RadioMessage = radioMessage;
    }
}