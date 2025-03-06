namespace PasswordGenerator;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

public partial class PasswordGeneratorForm : Form
{
    private Label lblPassword;
    private Button btnGenerateWithSymbols;
    private Button btnGenerateWithoutSymbols;
    private Label lblCopyStatus;

    public PasswordGeneratorForm()
    {
        InitializeComponent();

        this.Text = "安全密碼產生器";
        this.Size = new System.Drawing.Size(400, 200);

        lblPassword = new Label
        {
            Text = "按下按鈕來產生密碼",
            AutoSize = true,
            Location = new System.Drawing.Point(50, 20),
            Font = new System.Drawing.Font("微軟正黑體", 12),
        };

        btnGenerateWithSymbols = new Button
        {
            Text = "產生含特殊符號的密碼",
            Location = new System.Drawing.Point(50, 60),
            Width = 250,  // 修正：使用大寫 Width
        };
        btnGenerateWithSymbols.Click += BtnGenerateWithSymbols_Click;  // 修正：使用正確的事件處理方式

        btnGenerateWithoutSymbols = new Button
        {
            Text = "產生不含特殊符號的密碼",
            Location = new System.Drawing.Point(50, 100),
            Width = 250,  // 修正：使用大寫 Width
        };
        btnGenerateWithoutSymbols.Click += BtnGenerateWithoutSymbols_Click;  // 修正：使用正確的事件處理方式

        // 新增狀態標籤
        lblCopyStatus = new Label
        {
            Text = "",
            AutoSize = true,
            Location = new System.Drawing.Point(50, 140),
            Font = new System.Drawing.Font("微軟正黑體", 9),
            ForeColor = System.Drawing.Color.Green,
            Visible = false
        };

        this.Controls.Add(lblPassword);
        this.Controls.Add(btnGenerateWithSymbols);
        this.Controls.Add(btnGenerateWithoutSymbols);
        this.Controls.Add(lblCopyStatus);
    }

    private void BtnGenerateWithSymbols_Click(object sender, EventArgs e)
    {
        GeneratePassword(true);
    }

    private void BtnGenerateWithoutSymbols_Click(object sender, EventArgs e)
    {
        GeneratePassword(false);
    }

    private void GeneratePassword(bool includeSymbols)
    {
        int passwordLength = 16;
        string password = GenerateSecurePassword(passwordLength, includeSymbols);
        lblPassword.Text = password;

        try
        {
            Clipboard.SetText(password);
            lblCopyStatus.Text = "✓ 已自動複製密碼到剪貼簿";
            lblCopyStatus.ForeColor = System.Drawing.Color.Green;
            lblCopyStatus.Visible = true;

            // 修正：使用 Timer 控制狀態標籤的顯示時間
            Timer statusTimer = new Timer();
            statusTimer.Interval = 3000;  // 3 秒
            statusTimer.Tick += (sender, e) =>
            {
                lblCopyStatus.Visible = false;
                statusTimer.Stop();
                statusTimer.Dispose();
            };
            statusTimer.Start();
        }
        catch (Exception ex)
        {
            lblCopyStatus.Text = $"複製失敗：{ex.Message}";
            lblCopyStatus.ForeColor = System.Drawing.Color.Red;
            lblCopyStatus.Visible = true;
        }        
    }

    private string GenerateSecurePassword(int length, bool includeSymbols)
    {
        const string lowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        const string uppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";
        const string symbolChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        string allowedChars = lowercaseChars + uppercaseChars + numberChars;
        if (includeSymbols)
        {
            allowedChars += symbolChars;
        }

        // 使用更新的 RandomNumberGenerator 方法 (解決過時警告)
        byte[] randomBytes = new byte[length];
        RandomNumberGenerator.Fill(randomBytes);

        StringBuilder password = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            int randomIndex = randomBytes[i] % allowedChars.Length;
            password.Append(allowedChars[randomIndex]);
        }

        return password.ToString();
    }
}