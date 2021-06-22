import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import 'package:asc_time_tracker/models/time_log_model.dart';
import 'package:asc_time_tracker/utils/constants.dart';
import 'package:asc_time_tracker/widgets/current_log.dart';
import 'package:asc_time_tracker/widgets/action_list.dart';
import 'package:asc_time_tracker/widgets/timer.dart';

class HomeScreen extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Container(
        padding: const EdgeInsets.all(defaultPadding),
        child: SafeArea(
            // Needed to dodge intrusions from operating system
            child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
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
              CurrentLog(),
              ActionList(),
            ])),
      ),
    );
  }
}
