# Serial Wifi Onboarding

Wifi onboarding is the process of bringing a new device onto your wifi network. This is challenging
for very small devices becuase they generally have no UI beyond a simple button and light.

This code enables onboarding for .Net Micro Framework devices that are using the ESP8266 serial wifi chip.
This code nominally takes care of authentication and authorization, although you, the app author, need to
decide on how you want to authenticate (decide if the new device is genuine) and authorize (decide if the
new device is one you want to allow in). These can involve serial-number checks, checks against a cloud
service and numerous other methods.

Code is provided for two roles : the Onboardee, which is the new device wishing to enter your wifi network;
and the Onborder, which is the device which is coordinating the onboarding of new devices. 

The onboarder may have one or two serial wifi blocks, although two is desirable. Presumably, your onboarder is also
functioning as some sort of hub or central facility for your network. With one serial interface, it needs to
temporarily drop off the network to talk on the private network established by the onboardee, which would interrupt
service to existing already-onboarded devices. With two serial wifi blocks, the hub/onboarder can keep one interface
attached to the backbone network while using the second interface for ad-hoc connections to devices trying to complete
the onboarding process. The code, however, will work with either one or two ESP8266 blocks on your hub/onboarder
node.
