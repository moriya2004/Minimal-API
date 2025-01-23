using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// שליפת ה-Connection String מתוך משתנה סביבה
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__todo_db");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not found in environment variables.");
}

// הוספת ה-DbContext לשירותים
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.37-mysql")));

// הוספת CORS כדי לאפשר קריאות מאפליקציה חיצונית
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// הוספת Swagger (לתיעוד ה-API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// הפעלת CORS עבור כל הקריאות
app.UseCors("AllowAll");

// הפעלת Swagger בסביבת פיתוח
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

// מיפוי ה-Routes של ה-API

// שליפת כל המשימות
app.MapGet("/items", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});

// הוספת משימה חדשה (IsComplete יהיה ברירת מחדל false)
app.MapPost("/items", async (ToDoDbContext db, Item newItem) =>
{
    newItem.IsComplete = false; // ברירת מחדל
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{newItem.Id}", newItem);
});

// עדכון משימה קיימת
app.MapPut("/items/{id}", async (ToDoDbContext db, int id, Item updatedItem) =>
{
    var existingItem = await db.Items.FindAsync(id);
    if (existingItem is null) return Results.NotFound();
    
    existingItem.IsComplete = updatedItem.IsComplete;
    
    await db.SaveChangesAsync();
    return Results.Ok(existingItem);
});

// מחיקת משימה
app.MapDelete("/items/{id}", async (ToDoDbContext db, int id) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();
    
    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapGet("/",() => "TasksListServer API is running!");
app.Run();
