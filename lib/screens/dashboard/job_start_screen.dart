import 'dart:async';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'package:asc_time_tracker/models/time_log_model.dart';

class JobStartScreen extends StatefulWidget {
  const JobStartScreen({Key? key}) : super(key: key);

  @override
  _JobStartScreenState createState() => _JobStartScreenState();
}

class _JobStartScreenState extends State<JobStartScreen> {
  // Initialized in initState, but need late keyword to make it wait
  late Timer _everySecond;

  @override
  void initState() {
    super.initState();

    // Refreshes timer
    _everySecond = new Timer.periodic(Duration(seconds: 1), (Timer t) {
      if (mounted) {
        setState(() {});
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Time Page'),
      ),
      body: Container(
          alignment: Alignment.center,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              Consumer<TimeLogModel>(
                  builder: (context, timeLog, child) => Text(
                        timeLog.getTime(),
                        style: Theme.of(context).textTheme.headline4,
                      )),
              OutlinedButton(
                  onPressed: () {
                    var timeLog = context.read<TimeLogModel>();
                    timeLog.toggle();
                  },
                  child: Text('Start')),
              OutlinedButton(
                  onPressed: () {
                    var timeLog = context.read<TimeLogModel>();
                    timeLog.reset();
                  },
                  child: Text('Reset')),
            ],
          )),
    );
  }
}
