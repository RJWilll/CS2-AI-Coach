# CS2-AI-Coach

# Summary
All in one AI coach for Counter Strike 2. Uses game data, video input, and Google Gemini AI to find weaknesses in your CS2 gameplay.
After each round, the Coach generates a full match report listing bulletpoints to learn from the previous round. The coach measures
strategy, positioning, economy, and aiming to help you improve after every round.

# How to use
Using either the Visual Studio project solution or the release version, run the application, enter your Google Gemini API Key, and 
click 'Start Capture'. Coach tips are updated at the end of every round. All CS2 data is collected as you play with no other input 
needed from the player.

# How it Works
When you start the coach, two things starts to happen. First, CS2 screenshots start to populate a rotating queue to be used in 
later prompts. Second, GSI or Game State Integration handler starts checking if the round has ended. If the round has ended,
the current game state, including loadout, positon, money, etc and the current screenshot queue is added to a Gemini Prompt.
The response to this prompt is then rendered on the screen. Currently, to not have to pay for all prompts people have, players
are required to provide their own Google Gemini API Keys to use the app.

# Limitations and Future
Even though AI is a rapidly developing field, video processing is still fairly slow. This slow 
processing combined with not wanting to store an updating video in memory or storage, led to screenshots being used instead 
of video. The obvious limitation to screenshots is that it doesn't save everything in between, potentially missing vital infomation. 
Screenshots and image processing also involves memory and AI processing limitations, but most of all, images are very tolken expensive.
These combined reasons are why screenshots are limited quantity, even if more of them would provide better results.

As AI grows, providing full streaming video with the GSI report, and targeted coaching AI model would really fulfill the goals of this
project.