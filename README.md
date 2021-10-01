## Setup

Open the solution file and run the project. You can test the mobile interface using Chrome Devtools "Toggle device toolbar" in the top next to Elements.

See Phi Ton (<pton@ascmt.com>) for any help with this.

Structure:
- Javascript for site interactions and CSS in wwwroot
    - js/logs: handle filtering logs and async updates
    - js/submit: handle passing right data to the controller actions
    - js/timer: handle timers and keeping track of them
    - js/ui: site tour, log colors, etc.
- Areas: login/register custom pages and code
- Controllers: minimal code for displaying views
- Data: ApplicationDbContext contains the code for seeding test data for a db
- Models: all the logic for working with timelogs
- Views: various frontend displays
    - MainIndex contains the timers and IndexInfo
    - IndexInfo contains the filters and IndexLogs, IndexStats

## Testing

Asc Time Tracker Tests contains the tests for this project. Code coverage can also be displayed if Resharper is available.