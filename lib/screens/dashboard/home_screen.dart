import 'package:flutter/material.dart';

import 'package:asc_time_tracker/widgets/timer.dart';
import 'package:asc_time_tracker/widgets/cards/action_list.dart';
import 'package:asc_time_tracker/utils/constants.dart';

class HomeScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
        body: Container(
      padding: const EdgeInsets.all(defaultPadding),
      child: SafeArea(  // Needed to dodge intrusions from operating system
          child:
              Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: [
        Text(
          greeting,
          style: Theme.of(context).textTheme.headline3,
          textAlign: TextAlign.left,
        ),
        TimerText(),
        Divider(
          height: 25,
          thickness: 5,
        ),
        ActionList(),
      ])),
    ));
  }
}
