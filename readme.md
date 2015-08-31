## RIOT API CHALLENGE 2.0

### This is the repo for our entry on the challenge


Check out the demo here: http://carloseme.xyz/APIChallenge

First of all...

![ez](/ezwin.png)
EZ WIN GG

#### Who were the extraordinary people who made this app?

**Carlos Martinez** - CarlosEME -> LAN / VENEZUELA    
**Steffany Aldama** - Steffzor -> LAN / VENEZUELA

#### So what is this all about?

We wanted to study the usage for each item before and after their rework (from patch 5.13), we want to make clear that the data shown is not the whole set of data Rito gave us, we we're only able to get a part of it (around 80k matches, 40k before and 40k after rework) because of internet connection issues it was impossible for us to gather all the data.

The usage is being shown by percentage, the KDA and most damage are represented as average, so even though it is not the whole data, the numbers represent pretty much what happened to those items around the patchs. Also, we only used a list of main AP Champions, champions who depend of these items. We tried to get the stats that make the most sense for our category.

We would've like to have more time to add more exciting features to this, time runs fast, we did the best we could :P

#### How do you consider the experience of creating this app?

It was amazing, we learned so much from this, from how to make the app scale better with more data, to asynchronous programming (to make the requests to the API), we even found a bug on the chart library we are using! Thanks to Rito for making this possible!

#### Technologies?

ASP.Net MVC with C#, using SQL Server 2012 for our DBs together with Entity Framework as our ORM.

Frontend we're using AngularJS for almost everything, Bootstrap and JQuery to help make it look great!

#### What went wrong for you?

Apart from the fact that we couldn't gather all the data, basically because we left that for last, and then we had connection issues... I think the only thing that went wrong was the design of the code, I think we we're so obsessed on adding features that we didn't though in any design, and we did pay for it later. As a note for a next time, first of all we need to make a simpler idea, then maybe start from Test Driven Development and work from there.

------------------------------------------------------------------------------------------------------------

#### How to build the project

The solution is separated in two projects, APItemsWinRate contains the website and a database in which only the calculations are stored, this way it isn't heavy at all and can be easily stored in a server. InitialDataUpload contains the database which stores the matches, It's a console application, the program contains a boolean that when true, the application will download data from the files stored in a folder you specify. After you've stored the matches in the database, you set the boolean to false, then the program will make the analisys of all the data you've downloaded.

The files should be formated like this: {region}_info, ex: LAN_before

Before running them, you should first run the database migrations stored in the migrations folders of each project. 

The projects gather a key (your development or production key) from a file named Web.config (on the website) and App.config (on the console app), so you should create them and include your key like this:

	<configuration>
	    <appSettings>
	    	<add key="ApiKey" value="..." />
	    </appSettings>
	</configuration>

The easiest way to recreate those files is by reinstalling Entity Framework from Nuget.

You will also need to add a connection string for each project on these files, named "DefaultConnection" and "MatchesDbConnection" respectively.