# geoDVR
| ![](RackMultipart20210219-4-19dvuam_html_a614035a97f5ecf.jpg) | **geoDVR Ethernet Camera Target Location and Footprint Protocol**
_Last Updated: 2-19-21_ | **Contact Information:** 3307 South College Avenue Suite 211Fort Collins, CO 80525 |
| --- | --- | --- |

Version 1.3

Contact:

Web: [www.remotegeo.com](http://www.remotegeo.com/)

Sales: [sales@remotegeo.com](mailto:sales@remotegeo.com)

Support: [support@remotegeo.com](mailto:support@remotegeo.com)

Phone: +1 970-367-7808

Copyright and Use Agreement

© Copyright 2019, Remote GeoSystems, Inc. All Rights reserved. The Remote GeoSystems name and

logo and all related product and service names, including geoDVR and LineVision, and design marks and slogans are the trademarks, and service marks of Remote GeoSystems, Inc.

Before loading, downloading, installing, upgrading or using any Licensed Product of Remote GeoSystems, Inc., users must read and agree to the license terms and conditions outlined in the Remote GeoSystems, Inc. [End](https://www.remotegeo.com/terms/)

[User License Agreements](https://www.remotegeo.com/terms/) found at [https://www.remotegeo.com/terms/](https://www.remotegeo.com/terms/).

All data, specifications, and information contained in this publication are based on information that we

believe is reliable at the time of printing. Remote GeoSystems, Inc. reserves the right to make changes

without prior notice.

Revision History

_Version 1.0 – 10-22-18_

_Version 1.1 – 10-30-18_

_Version 1.2 – 11-22-18_

_Version 1.3 – 3-5-19_

_Version 1.4 – 2-19-21_

1. **Overview**

The Remote GeoSystems, Inc&#39;s geoDVR Ethernet Camera Target Location and Footprint Protocol defines the connectivity and data format requirements in order for the Remote Geo geoDVR to collect the required variables for displaying the camera target location and the camera footprint on the map when being used in the Remote Geo LineVision software suite. Additionally, the protocol also allows the geoDVR Live Moving Maps module to display the camera target location along with the platform&#39;s (aircraft, vehicle, etc) location. Once collected and imported into LineVision, the data is presented in a format similar to the following:

![](RackMultipart20210219-4-19dvuam_html_5a6f1bc7c9c95f28.png)

_ **IMPORTANT NOTE:** _ The variables outlined in the Ethernet Camera Target Location and Footprint Protocol can also be captured and displayed in LineVision via the MISB STANAG protocol. LineVision can handle both the MISB protocol and the protocol outlined in this document. However, the purposed of this specification is to provide an alternate, greatly simplified protocol as compared to MISB that provides the minimum required variables for showing both the target location and camera footprint on the map. _The coordinate system and order of operations used in the protocol outlined in this document parallels the ones used in the MISB specification._

1. **geoDVR Module Requirements**

The following geoDVR modules are required for the protocol to collect and display the required data:

1. Live Moving Maps module
2. Camera Target Footprint module _(Note: This has also previously been referred to in some documents as the Advanced Gimbal Targeting &amp; 4-Corner MISB FMV module)_

1. **Hardware &amp; Software Requirements**

To implement the Ethernet Camera Target Location and Footprint Protocol outlined in this document, the device sending the data to the geoDVR must have an Ethernet port available that can send TCP data over a specified port. By default, the geoDVR expects the data to be sent over port 1121. This port can be changed via a configuration setting in the geoDVR, although it is recommended that the default port be used. The device sending the information must have any firewalls, etc. configured to allow two-way communication over the default port. The device sending the information must also have a static IP address that can be entered into the geoDVR. It is recommended that the sending device be on the 192.168.0.1 subnet with a static IP address of 192.168.0.20. The geoDVR automatically &quot;listens&quot; to this port/IP/subnet configuration although the configuration can be changed via a configuration setting in the geoDVR.

_In summary:_

Recommendations for devices implementing and sending protocol data:

- Recommended Port: 1121
- Recommended IP address: 192.168.0.20
- Recommended Subnet: 192.168.0.1
- Ethernet Protocol: TCP/IP

1. **Protocol Specification**

The data being sent via the protocol should be sent in constant intervals up to a maximum of 5 Hz. Higher data frequencies are allowed although they have not been tested. In general, the recommended frequency is 1 Hz. The minimum recommended frequency is also 1 Hz.

The data being sent should be XML &quot;packets&quot; resembling the following:

_(NOTE: This example contains more than the minimum required fields. The minimum required fields are outlined below)_

\&lt;trkpt\&gt;

\&lt;lat\&gt;40.561952\&lt;/lat\&gt;

\&lt;lon\&gt;-105.116984\&lt;/lon\&gt;

\&lt;ele\&gt;28.245\&lt;/ele\&gt;

\&lt;time\&gt;2018-11-22T13:45:30.0000000-07:00\&lt;/time\&gt;

\&lt;course\&gt;155.652455\&lt;/course\&gt;

\&lt;speed\&gt;1.734\&lt;/speed\&gt;

\&lt;fix\&gt;2\&lt;/fix\&gt;

\&lt;sat\&gt;21\&lt;/sat\&gt;

\&lt;hdop\&gt;0\&lt;/hdop\&gt;

\&lt;platform\_pitch\&gt;0.138795\&lt;/platform\_pitch\&gt;

\&lt;platform\_roll\&gt;-0.365271\&lt;/platform\_roll\&gt;

\&lt;platform\_heading\&gt;155.69548\&lt;/platform\_heading\&gt;

\&lt;sensor\_relative\_azimuth\&gt;15.65241\&lt;/sensor\_relative\_azimuth\&gt;

\&lt;sensor\_relative\_pitch\&gt;-39.603579\&lt;/sensor\_relative\_pitch\&gt;

\&lt;sensor\_relative\_roll\&gt;0\&lt;/sensor\_relative\_roll\&gt;

\&lt;slant\_range\&gt;0\&lt;/slant\_range\&gt;

\&lt;frame\_lat\&gt;40.56218547\&lt;/frame\_lat\&gt;

\&lt;frame\_lon\&gt;-105.11660662\&lt;/frame\_lon\&gt;

\&lt;frame\_ele\&gt;0.086115802687093082\&lt;/frame\_ele\&gt;

\&lt;horizontal\_fov\&gt;21.2300\&lt;/horizontal\_fov\&gt;

\&lt;/trkpt\&gt;

1. **Variable Definitions**

| **Variable** | **Description** | **Units** |
| --- | --- | --- |
| trkpt | This is the XML packet&#39;s &quot;root&quot; element and must be included in all packets being sent. | N/A |
| lat | The latitude in decimal degree format. At least six significant figures are recommended. | Decimal Degrees |
| lon | The longitude in decimal degree format. At least six significant figures are recommended. | Decimal Degrees |
| ele | The altitude, in meters, above sea level. At least three significant figures are recommended. | Meters above sea level |
| time | GPS UTC Time in yyyy-MM-ddTHH:mm:ss.fffffffK format.
The format is the &quot;Round-trip date/time pattern&quot; as defined by &quot;o&quot; and &quot;O&quot; at [https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings) | yyyy-MM-ddTHH:mm:ss.fffffffKformat with&quot;y&quot; = year&quot;M&quot; = month&quot;d&quot; = day&quot;H&quot; = 24 hour&quot;m&quot; = minute&quot;s&quot; = second&quot;f&quot; = milliseconds&quot;K&quot; = Time zone |
| course | The course of the platform as true direction (relative to true north). At lease six significant figures are recommended. | Degrees |
| speed | The speed of the platform in knots. At least three significant figures are recommended. | Knots |
| fix | GPS fix quality per the NMEA spec.
Fix quality: 0 = invalid1 = GPS fix (SPS)2 = DGPS fix3 = PPS fix4 = Real Time Kinematic 5 = Float RTK6 = estimated (dead reckoning) (2.3 feature) 7 = Manual input mode 8 = Simulation mode | Integer |
| sat | The number of satellites used in GPS calculations. | Integer |
| hdop | Horizontal dilution of precision. | Decimal number |
| platform\_pitch | See figure 1. The platform pitch in degrees. At least six significant figures are recommended. | Degrees |
| platform\_roll | See figure 1. The platform roll in degrees. At least six significant figures are recommended. | Degrees |
| platform\_heading | See figure 1. The platform heading in degrees as true direction. At least six significant figures are recommended. | Degrees |
| sensor\_relative\_azimuth | See figure 1. The sensor (camera direction) azimuth relative to the platform. At least six significant figures are recommended. | Degrees |
| sensor\_relative\_pitch | See figure 1. The sensor (camera) pitch relative to the platform. At least six significant figures are recommended. | Degrees |
| sensor\_relative\_roll | The sensor (camera) roll relative to the platform. At least six significant figures are recommended. | Degrees |
| slant\_range | The distance between the sensor and the ground intersection. At least three significant figures are recommended. | Meters |
| frame\_lat | The target location latitude as decimal degrees. At least six significant figures are recommended. | Decimal Degrees |
| frame\_lon | The target location longitude as decimal degrees. At least six significant figures are recommended. | Decimal Degrees |
| frame\_ele | The target altitude, in meters, above sea level. At least three significant figures are recommended. | Meters above sea level |
| horizontal\_fov | The horizontal field-of-view of the camera(s). At least six significant figures are recommended. | Degrees |

_ **IMPORTANT: The angles and variables mentioned above MUST follow the coordinate system defined in the MISB specification. Please refer to Figure 1 below.** _

![](RackMultipart20210219-4-19dvuam_html_6b5bf934e8b50105.png) ![](RackMultipart20210219-4-19dvuam_html_7d8579bb9bb0bf6a.png)

**Figure 1**

1. **Minimum Required Variables**

If frame center (sensor target location) calculations are present, the minimum variables required are the following:

\&lt;trkpt\&gt;

\&lt;lat\&gt;40.561952\&lt;/lat\&gt;

\&lt;lon\&gt;-105.116984\&lt;/lon\&gt;

\&lt;ele\&gt;28.245\&lt;/ele\&gt;

\&lt;time\&gt;2018-11-22T13:45:30.0000000-07:00\&lt;/time\&gt;

\&lt;course\&gt;155.652455\&lt;/course\&gt;

\&lt;speed\&gt;1.734\&lt;/speed\&gt;

\&lt;frame\_lat\&gt;40.56218547\&lt;/frame\_lat\&gt;

\&lt;frame\_lon\&gt;-105.11660662\&lt;/frame\_lon\&gt;

\&lt;frame\_ele\&gt;0.0861158026870\&lt;/frame\_ele\&gt;

\&lt;horizontal\_fov\&gt;21.2300\&lt;/horizontal\_fov\&gt;

\&lt;/trkpt\&gt;

Provided the frame center calculations are present, at a minimum the horizontal field-of-view variable (hfov1) from one camera (typically the TV camera) needs to be provided along with the &quot;standard&quot; platform GPS data. Using this data, LineVision will dynamically calculate the vertical field-of-view based on the recorded video&#39;s aspect ratio.

1. **geoDVR Configuration**

To configure the geoDVR to receive GPS data from your gimbal/device, navigate to the File Menu and select &quot;Options-\&gt;Use Remote Geo Ethernet GPS&quot; from the drop-down menu. This setting configures the geoDVR to listen for incoming connections (it behaves like a server) and the &quot;ANY&quot; address resolves to 0.0.0.0 which allows connections from any client.

![](RackMultipart20210219-4-19dvuam_html_f0a76f1d685c9ed9.png)

1. **Camera Control**

The geoDVR can be used to control your gimbal. If enabled, they the map is touched (clicked), the location touched is sent over a network connection. The data packet containing the location coordinates where the map was &quot;touched&quot; takes on the form:

\&lt;GPStouch\&gt;

\&lt;lat\&gt;\&lt;/lat\&gt;

\&lt;lon\&gt;\&lt;/lon\&gt;

\&lt;/GPStouch\&gt;

In the above, \&lt;lat\&gt; is a numeric value representing the latitude and \&lt;lon\&gt; is a numeric value representing the longitude.

To receive the coordinates, two methods are available. The first option is by using the existing TCP Ethernet connection that is sending the GPS/frame center coordinates to the geoDVR (as configured above when &quot;Options-\&gt;Use Remote Geo Ethernet GPS&quot; is selected from the main menu).  When the coordinates are sent to the geoDVR, a two-way TCP connection is opened and whenever the map is touched (clicked), the above xml packet is passed back over the connection and the specified port.

The second option is by listening to a UDP Connection that is broadcast from the geoDVR.  The UDP packets also take on the same format as above and are configured when &quot;Moving Maps-\&gt;Enable Camera Control&quot; is selected from the main menu (IMPORTANT: &quot;Enable Camera Control&quot; must be selected for camera control coordinates to be sent):

![](RackMultipart20210219-4-19dvuam_html_1cc0728beb5a8c95.png)

![](RackMultipart20210219-4-19dvuam_html_28cbe116c5730cd0.png)

In this case, the IP Address specified must be the IP address of the gimbal/computer where the camera control coordinates are being sent.

Once the above are configured, a button towards the top of the map labeled &quot;Enable Camera Positioning on Click&quot; should appear.

![](RackMultipart20210219-4-19dvuam_html_d7bad52dd163dcfd.png)

When selected, camera control coordinates will be sent over both the TCP and UDP connections. IMPORTANT: Only one connection, either TCP or UDP, is required for gimbal control. The two different options are offered to give more flexibility in programming options.

When touched (clicked), the text should change to &quot;Disable Camera Positioning on Click&quot; and the button will be highlighted yellow.

![](RackMultipart20210219-4-19dvuam_html_a9e26022ac18c49f.png)

When selected, each time you touch the map, it will send a data point on that exact spot.

1. **Protocol Test Application, Example Source Code**

To assist with sending GPS to the geoDVR and to test Camera Control connection, source code has been placed on the Remote GeoSystems GitHub account. A test application can be downloaded from:

[https://github.com/RemoteGeoSystems/geoDVR/tree/geoDVR/RemoteGeoProtocolXmlTestApp/RemoteGeoProtocolXmlTestApp/bin/Release](https://github.com/RemoteGeoSystems/geoDVR/tree/geoDVR/RemoteGeoProtocolXmlTestApp/RemoteGeoProtocolXmlTestApp/bin/Release)

The source code for the application can be downloaded from the same repository at:

[https://github.com/RemoteGeoSystems/geoDVR/tree/geoDVR/RemoteGeoProtocolXmlTestApp](https://github.com/RemoteGeoSystems/geoDVR/tree/geoDVR/RemoteGeoProtocolXmlTestApp)

To run the source code, Visual Studio 2019 is recommended. The source code is provided in C#.

1. **Questions and Additional Support**

For questions or additional support, please contact the support email address listed above or visit the Remote GeoSystems, Inc. website at [www.remotegeo.com](http://www.remotegeo.com/) or visit the Remote Geo Product Support forums at [https://support.remotegeo.com/forums/](https://support.remotegeo.com/forums/).

The geoDVR webpage is located at [https://www.remotegeo.com/geodvr/](https://www.remotegeo.com/geodvr/).

LineVision information can be found at [https://www.remotegeo.com/software/linevision-desktop/](https://www.remotegeo.com/software/linevision-desktop/).
