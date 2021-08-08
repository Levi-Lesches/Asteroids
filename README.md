## Asteroids -- with AimBot

A remake of Atari's Asteroids, with an "AimBot", that can take control and play the game for you!

While playing the _amazing_, high-quality, and endlessly entertaining [Asteroid Storm](https://play.google.com/store/apps/details?id=com.ZephirothGames.AsteroidStorm) (free version [here](https://play.google.com/store/apps/details?id=com.ZephirothGames.AsteroidStormFREE)), I had a go-to strategy: stay in the middle and one-shot the moving asteroids. Then I realized that, like all good things, my playstyle could be automated. And so I made this game. 

Because I wanted to focus on the AimBot, this is a standard Unity project. No special 3D models, art, or plugins. The graphics are barebone (but cute, at least to me), and the extent to which features are replicated is minimal. It's still playable without the AimBot, but if you're looking for a fun time, download the Asteroid Storm app. This project is just to test my kinematics skills. 

### The code

All the scripts are in the `Assets\Scripts` folder. There's a `GameController` which controls everythinig about the state of the game, and several other smaller scripts: 

- `AimBot`, the whole point of this project. See the section on that below. 
- `Border`, transports entities across the border so it looks like they wrap around
- `Destructible`, anything that can be destroyed on contact has this script
- `Enemy`, makes the enemy ships move acros the screen and shoot the player
- `HUDController`, exposes an API for `GameController` to manage the HUD
- `Projectile`, kills the projectile after a certain time (so it doesn't fly around)
- `ShipController`, handles user input to control the ship

## How AimBot works

AimBot works in a simple 8-step process: 

1. Choose the nearest target, prioritizing enemy ships over asteroids) (`GetNearestTarget`)
2. Rotate to face the target (`FixedUpdate`)
3. Choose the nearest target, in case it changed since step 1 (`GetNearestTarget`)
4. Calculate a point along the target's trajectory that a projectile shot would reach in the same time the target would. This is the _interception point_ (`GetIntercept`)
5. Rotate to face the interception point. If the target is the same as step 1, this will be very quick, as the ship is already facing the target's current position. (`FixedUpdate`)
6. Shoot and repeat. (`FixedUpdate`)

To expand a bit, `GetIntercept` works by plotting out `n` points over the next `T` seconds along the target's trajectory such that each point is spaced `n / T` seconds away. At each of these points, it calculates the time a projectile shot would take to reach that point. We already know the time it takes for the target to reach that point, because each point is spaced evenly along the trajectory. If the ETA of the projectile and the ETA of the target are within a certain threshold, then the function returns that position. 

Then, another function `GetRotation` calculates the Quaternion (rotation) needed to face the point returned by `GetIntercept`. To simulate player movement, the ship turns a little bit using `Quaternion.Slerp` in `FixedUpdate` until the ship is fully rotated. Then, AimBot simulates a shot using `ShipController.Fire`, exactly as if the user had manually fired. 

## Contributing

I didn't really mean for this project to become anything more than a fun demo, but if you're interested in moving it forward, feel free to open PRs and fork the project. 

`.todo` is my personal to-do list. Since I'm not working on it anymore, feel free to start there. 