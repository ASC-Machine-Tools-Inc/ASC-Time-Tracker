import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

class TimerText extends StatefulWidget {
  const TimerText({Key? key}) : super(key: key);

  @override
  _TimerTextState createState() => _TimerTextState();
}

class _TimerTextState extends State<TimerText> {
  String _now = DateFormat.yMd().add_jms().format(DateTime.now());
  // Initialized in initState, but need late keyword to make it wait
  late Timer _everySecond;

  @override
  void initState() {
    super.initState();

    // Defines a timer
    _everySecond = new Timer.periodic(Duration(seconds: 1), (Timer t) {
      if (mounted) {
        setState(() {
          // Gets the current time in format MM/dd/yyyy hh:mm:ss a
          // Example: 6/21/2021 3:18:46 PM
          _now = DateFormat.yMd().add_jms().format(DateTime.now());
        });
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Text(_now, style: Theme.of(context).textTheme.subtitle1);
  }
}
