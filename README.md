# WeirdCardGame

///     todo: package the solution by cloning to a different path!

--------------------------------------------------------------------------------
The main purpose of the code test is to see your methodology and approach to
problem solving and delivery on a project. The main priority is not a completed,
client ready project, rather an end to end working project that showcases your
skills approach to software development.

For example, do not spend too much time perfecting the UI, or you could prefer
to do that and mock the data store.

Just ensure you document your approach in an accompanying readme.
--------------------------------------------------------------------------------

Notes on how the solution was created
--------------------------------------------------------------------------------
- This solution was created in VS2017 community edition.

- It is an ASP.NET Core web application named WeirdCardGame.
	- It was created using a VS2017 template and probably has excess content.
	- I removed some of the excess content but probaby not all of it.

- It makes use of Entity Framework Core for an in memory db.
	- It does this when running as the web app and in unit tests.

- It makes use of nUnit and Moq for unit testing.
	- Note: I was going to use Moq but ended up using Fake services to test the
	GamePlayingService as I found it easier to set it all up that way.
--------------------------------------------------------------------------------

Notes on what I chose to focus on:
--------------------------------------------------------------------------------
I decided to try and build a web app with good UX as this can be hard to gauge
in conversation. There wasn't too much scope to go crazy with it and that's good.

I thought it would be useful to render the cards as playing cards, so I did that.
I chose to show the rule cards as cards with an unknown suit with question marks
where the suit or kind of the card was unknown. I think it works pretty well. I
think that it would be worth adding tool tips on the cards so that any one who
decides they are unsure what the ? means can get a brief explanation of it. All
in all I think that showing rule cards as playing cards works better than using
either a list or table of card kinds and values.

I am less familiar with recent versions of Angular but I thought I would take on
the challenge of learning it a little while doing the code test at the same time.

I know that I could architect the client side to be more resilient by using
service classes to talk with the server instead of putting so much code in the
one component class.

I didn't get around to making a special component for rendering the playing card
on the client but it is one obvious improvement since it is a repeated pattern.

I allowed server side errors to make their way to the client so that they could
be displayed to the user. I chose not to log as it is not production code but I
would log errors to Seq via Serilog given the chance.

****
Because I had some time but not oodles of time I decided to make use of Bootstrap
as a basic framework for visual styling. I'm not a Bootstrap expert but I think
what I've done for the game hangs together pretty well.

****
I decided late on that there was no point storing results in the db if I wasn't
going to show them somewhere so I added an MVP Winners List that lists them in
reverse chronological order.

****
I took some short cuts on my modelling. Card should exist with a Kind and Suit
but no Score. There should be a ScoredCard class that inherits from Card and
adds the Score.

****
Early on it became obvious that there could be a tie for first place. I chose
to say that since this result has no clear winner then it should have no winner.
This means no winner id stored in the db, and it is shown differently in the UI.

--------------------------------------------------------------------------------
