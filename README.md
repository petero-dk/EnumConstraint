# EnumConstraint
A set of `IRouteConstraint` that functions on EnumValues and the name value of the `DisplayAttribute` on EnumValues for really nice urls

## Installation

Install package from nuget `Sustainable.Web.Mvc.EnumConstraint`

## EnumDisplayNameConstraint

How to have pretty urls with the slug determined by the *display* name of the enum value.

Having an enum like this

```csharp
    public enum EventTypeEnum
    {

        [Display(Name = "MyEvent", Description = "Events just for me")]
        SingleEvent,
        [Display(Name = "OurEvents", Description = "Events for us")]
        MultipleEvents,
        [Display(Name = "Everybody", Description = "Events for everybody")]
        All,
    }
```

and an action on a controller like this

```csharp
    public async Task<ActionResult> Details(EventTypeEnum type, string id)
    {
        // Notice how the parameter is the enum not a text field that you have to parse
        return View();
    }
```

It is possible to have urls that look like this

`https://sample.com/Events/OurEvents/FirstEventId`

By setting the route information like this

```csharp
    app.UseMvc(routes =>
            {
                routes.MapRoute("events", "Events/{type}/{id}",
                        new { Controller = "Events", Action = "Details" },
                        new { type = new EnumDisplayNameConstraint<Models.Data.EventTypeEnum>(true)} 
                );

                // Default mvc route
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
```


## EnumValueConstraint

How to have pretty urls with the slug determined by the enum *value*.

TODO