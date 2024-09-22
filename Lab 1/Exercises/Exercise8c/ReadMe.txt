// Add GestureDetector.cs to your Solution. Open GestureDetector.cs and change the namespace to match the namespace of your form file.

// Add a GestureDetector object to the top of your form:
GestureDetector detector = new GestureDetector();

// When stripping your concurrent queue, you now need to feed the detector:
// Example code
int nextVal;
bool success = dataQueue.TryDequeue(out nextVal);
while (success)
{
    detector.FeedSerialValue(nextVal);

    success = dataQueue.TryDequeue(out nextVal);
}

// The detector has lots of handy functions you can call at any time. Many of them return a type called Vec3 which has 3 components. For example:
Vec3 example = new Vec3();
float x = example.X;

// Last acceleration functions
detector.GetLatestAcceleration_SI() // Latest acceleration vector in SI units
detector.GetLatestAcceleration_RAW() // Latest acceleration vector in raw values


// Average acceleration functions
detector.SetAveragingCount(int count) // Set how many n previous samples to average
detector.GetAverageAcceleration_SI() // Average acceleration vector in SI units
detector.GetAverageAcceleration_RAW() // Average acceleration vector in raw values

// Gesture detection functions
detector.EnableFastDetection(bool fasterDetection) // Optional to speed up detection
detector.IgnoreChannels(char[] channels) // Example to ignore the negative channels: detector.IgnoreChannels(new char[] { 'A', 'B', 'C' });
detector.GetPartialGesture() // Get the detected movements so far as a string. Don't use this for gesture detection, use PopMostRecentGesture()

// Detecting specific gestures:
// Example code, run within a fast timer
string gesture = detector.PopMostRecentGesture();
if (gesture.Equals(""))
	//Nothing yet
else if (gesture.Equals("XCYA"))
	// Equivalent to +X, -Z, +Y, -X