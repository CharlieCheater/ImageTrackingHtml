using System.Text;

var curDomain = AppDomain.CurrentDomain.BaseDirectory;
var path = "";
bool isHorizonatal = false;
DirectoryInfo di = null;
do
{

    Console.WriteLine("Укажите путь до директории с изображениями");
    Console.Write("Path: ");
    path = Console.ReadLine();
    di = new DirectoryInfo(path);
    if (!di.Exists)
    {
        Console.WriteLine("Директория не существует");
    }
} while (!di.Exists);

var imageUrl = "";
var diskPath = "https://disk.yandex.ru/client/disk/";
Console.WriteLine("Укажите сетевой адрес до папки с изображением");
Console.Write("Web Path: ");
imageUrl = Console.ReadLine();
Console.Write("Horizontal Images? (Y/n): ");
isHorizonatal = Console.ReadLine().ToLower().Any(x => x == 'y' || x == 'н');

var tdStyle = isHorizonatal ? "horizontal-td" : "vertical-td";
var imgStyle = isHorizonatal ? "horizontal-img" : "vertical-img";
var columns = 3;
var maxItemsPerPage = isHorizonatal ? 15 : 3;

StringBuilder htmlBuilder = new StringBuilder();
var extensions = new string[] { ".jpg", ".jpeg", ".png", ".svg" };
var images = di.GetFiles("*.*",
                 SearchOption.TopDirectoryOnly)
                .Where(file => extensions.Contains(file.Extension))
                .ToList();

htmlBuilder.Append("<html><body>");
htmlBuilder.AppendLine($"<style>{File.ReadAllText(Path.Combine(curDomain, "styles.css"))}</style>");
htmlBuilder.Append("<table>");
int count = 0;
for (int i = 0; i < images.Count(); i++)
{
    if (count % columns == 0)
    {
        htmlBuilder.Append("<tr>");
    }
    var image = images[i];
    htmlBuilder.Append($"<td class='{tdStyle}'>");
    htmlBuilder.Append($"<div style='text-align:center;'><img class='{imgStyle}' src='data:image/png;base64, {GetBase64(image.FullName)}' /></div><br>");
    htmlBuilder.Append($"Название: {image.Name}<br>");
    htmlBuilder.Append($"Ссылка: <a href='{diskPath + imageUrl}'>{diskPath + imageUrl}");
    htmlBuilder.Append("</td>");

    count++;
    if (count % columns == 0)
    {
        htmlBuilder.Append("</tr>");
    }
    if (count % maxItemsPerPage == 0)
    {
        htmlBuilder.Append("</table><table>");
    }
}

htmlBuilder.Append("</table>");
htmlBuilder.Append("</body></html>");

File.WriteAllText(Path.Combine(curDomain, $"output {DateTime.Now.ToFileTime()}.html"), htmlBuilder.ToString());

static string GetBase64(string imgPath)
{
    byte[] imageArray = System.IO.File.ReadAllBytes(imgPath);
    return Convert.ToBase64String(imageArray);
}