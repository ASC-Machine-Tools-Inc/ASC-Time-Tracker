import 'dart:async';

import 'package:flutter/material.dart';
import 'package:sizer/sizer.dart';
import 'package:provider/provider.dart';

import 'package:asc_time_tracker/models/time_log_model.dart';
import 'package:asc_time_tracker/utils/constants.dart';

class CurrentLog extends StatefulWidget {
  const CurrentLog({Key? key}) : super(key: key);

  @override
  _CurrentLogState createState() => _CurrentLogState();
}

class _CurrentLogState extends State<CurrentLog> {
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
    return SizedBox(
      // Manages size of card
      width: cardWidth.w,
      height: cardHeight.h,
      child: GestureDetector(
        // Registers tap action on card
        onTap: () => Navigator.pushNamed(context, '/'),
        child: Card(
            child: Container(
              decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(cardCorners),
                  gradient: LinearGradient(
                    begin: Alignment.topRight,
                    end: Alignment.bottomLeft,
                    colors: [
                      Color(0xFF4E4376),
                      Color(0xFF2B5876),
                    ],
                  )),
              padding: const EdgeInsets.symmetric(
                  vertical: vertCardPadding, horizontal: defaultPadding),
              child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Icon(Icons.schedule),
                    Consumer<TimeLogModel>(
                        builder: (context, timeLog, child) => Text(
                          timeLog.isZero() ? 'No log \ncurrently active' : timeLog.getTime(),
                          style: Theme.of(context).textTheme.headline5,
                          textAlign: TextAlign.right,
                        )),
                  ]),
            )),
      ),
    );
  }
}