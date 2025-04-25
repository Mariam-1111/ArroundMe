using first.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace first.Pages
{
    public class SignInModel : PageModel
    {
        public string msg { get; set; } = string.Empty;

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost(string Username, string Password)
        {
            // יצירת פרמטרים לשאילתת ה-SQL
            string SQLStr = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
            Helper helper = new Helper();

            // יצירת פרמטרים לשאילתת ה-SQL בצורה מאובטחת
            var parameters = new Dictionary<string, object>
            {
                { "@Username", Username },
                { "@Password", Password }
            };
           

            DataTable dt = helper.RetrieveTable(SQLStr, "Users", parameters);

            if (dt.Rows.Count > 0)
            {
                HttpContext.Session.SetString("Login", Username);

                // קבלת הערך פעם אחת בצורה בטוחה
                string fullName = $"{dt.Rows[0]["FirstName"]} {dt.Rows[0]["LastName"]}";
                HttpContext.Session.SetString("FullName", fullName);

                // קבלת הערך של "UserAdmin"
                object rawValue = dt.Rows[0]["UserAdmin"];
                bool isAdmin = rawValue != DBNull.Value && Convert.ToBoolean(rawValue);

                // שמירה ב-Session של סטטוס המנהל
                HttpContext.Session.SetString("Admin", isAdmin.ToString());

                // ניתוב בהתאם אם המשתמש הוא מנהל
                if (isAdmin)
                {
                    return RedirectToPage("../Users/Index");
                }
                else
                {
                    return RedirectToPage("/Content/My account");
                }
            }


            msg = "Wrong username or password.";
            return Page();
        }
    }
}