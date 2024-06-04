using Application;
using Application.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbConnectionFactory(builder.Configuration["DbConnection"]);
builder.Services.AddEmailHandler(builder.Configuration["ResendAPI"]);
builder.Services.AddIdentity();

builder.Services.AddServices();
builder.Services.AddViewModels();

builder.Services.AddAntDesign();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AuthenticationMiddleware>();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
