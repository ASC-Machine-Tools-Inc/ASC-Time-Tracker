import 'package:flutter/material.dart';
import 'package:sizer/sizer.dart';

import 'package:asc_time_tracker/utils/constants.dart';

class ActionList extends StatelessWidget {
  const ActionList({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
        shrinkWrap: true,
        itemCount: card_titles.length,
        itemBuilder: (context, index) {
          return SizedBox(
            // Manages size of card
            width: cardWidth.w,
            height: cardHeight.h,
            child: GestureDetector(
              // Registers tap action on card
              onTap: () => Navigator.pushNamed(context, routes[index]),
              child: Card(
                  child: Container(
                decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(cardCorners),
                    gradient: LinearGradient(
                      begin: Alignment.topRight,
                      end: Alignment.bottomLeft,
                      colors: [
                        colors[index][0],
                        colors[index][1],
                      ],
                    )),
                padding: const EdgeInsets.symmetric(
                    vertical: vertCardPadding, horizontal: defaultPadding),
                child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Icon(icons[index]),
                      Text(
                        card_titles[index],
                        style: Theme.of(context).textTheme.headline5,
                        textAlign: TextAlign.right,
                      ),
                    ]),
              )),
            ),
          );
        });
  }
}
