using Azure;
using Refit;

namespace HackerNews;

public sealed partial class MainPage : Page
{

	NewsViewModel Vm => (NewsViewModel)DataContext;


	public MainPage()
	{
		this.InitializeComponent();
		DataContext = CreateVMAndResources();
		this.listView.Tapped += (s,e ) => 
		{
			var list = (ListView)s;
			var selectedItem = list.SelectedItem;

			if (selectedItem is null)
				return;
		};
	}


	NewsViewModel CreateVMAndResources()
	{
		var textClient = new Azure.AI.TextAnalytics.TextAnalyticsClient(new(TextAnalysisConstants.BaseUrl), new AzureKeyCredential(TextAnalysisConstants.SentimentKey));
		var client = new HttpClient
		{
			BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0")
		};

		var refitBla = RestService.For<IHackerNewsAPI>(client);

		var hackerApis = new HackerNewsAPIService(refitBla);
		
		return new NewsViewModel(new TextAnalysisService(textClient), hackerApis);
	}


	protected override async void OnNavigatedTo(NavigationEventArgs e)
	{
		base.OnNavigatedTo(e);

		await Vm.RefreshCommand.ExecuteAsync(null);
	}
}
