import 'package:flutter/cupertino.dart';

class TimeLogModel extends ChangeNotifier {
  // Internal, private current time.
  Stopwatch _stopwatch = new Stopwatch();

  void toggle() {
    if (_stopwatch.isRunning) {
      _stopwatch.stop();
    } else {
      _stopwatch.start();
    }
    notifyListeners();
  }

  void reset() {
    _stopwatch.stop();
    _stopwatch.reset();
    notifyListeners();
  }

  String getTime() {
    var secs = _stopwatch.elapsedMilliseconds ~/ 1000;
    var hours = (secs ~/ 3600).toString().padLeft(2, '0');
    var minutes = ((secs % 3600) ~/ 60).toString().padLeft(2, '0');
    var seconds = (secs % 60).toString().padLeft(2, '0');

    return "$hours:$minutes:$seconds";
  }

  bool isZero() {
    return _stopwatch.elapsed.inMicroseconds == 0;
  }
}
