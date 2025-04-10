namespace AngularAuthApi.Helpers
{
    public static class EmailBody
    {
        public static string EmailStringBody(string email, string emailToken)
        {
            return $@"<html>
       <head>
    <title>Reset Your Password</title>
    <style>
        /* Global Styles */
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Arial', Helvetica, sans-serif;
        }}

        body {{
            background-color: #f3f6f9;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
        }}

        /* Container */
        .email-container {{
            background: #fff;
            width: 100%;
            max-width: 480px;
            padding: 40px;
            border-radius: 8px;
            box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
        }}

        /* Heading */
        h1 {{
            font-size: 24px;
            color: #333;
            margin-bottom: 20px;
            text-align: center;
        }}

        /* Paragraph Styling */
        p {{
            color: #555;
            font-size: 16px;
            margin-bottom: 20px;
            line-height: 1.5;
        }}

        /* Link Button */
        .reset-button {{
            background-color: #007bff;
            color: #fff;
            border-radius: 4px;
            display: block;
            width: 100%;
            max-width: 220px;
            margin: 0 auto;
            text-align: center;
            text-decoration: none;
            padding: 12px;
            font-size: 16px;
            font-weight: bold;
            transition: background-color 0.3s ease;
        }}

        .reset-button:hover {{
            background-color: #0056b3;
        }}

        /* Footer */
        .footer {{
            text-align: center;
            margin-top: 20px;
            font-size: 14px;
            color: #777;
        }}

        .footer a {{
            color: #007bff;
            text-decoration: none;
        }}

        .footer a:hover {{
            text-decoration: underline;
        }}

        /* Border Bottom */
        hr {{
            border: 0;
            border-top: 1px solid #eee;
            margin: 20px 0;
        }}
    </style>
</head>
<body>

    <div class=""email-container"">
        <h1>Reset Your Password</h1>
        <hr>
        <p>You're receiving this email because you requested a password reset for your Healthcare Plus account.</p>
        <p>Please click the button below to choose a new password.</p>
        <a href=""http://localhost:4200/reset?email={email}&code={emailToken}"" 
           target=""_blank"" 
           class=""reset-button"">
            Reset Password
        </a>
        <p class=""footer"">Kind Regards,<br><br>Healthcare Plus Team</p>
    </div>

</body>
</html>
";
        }

    }
}
