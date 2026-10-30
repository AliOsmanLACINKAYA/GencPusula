import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:genc_pusula/core/widgets/logo.dart';

void main() {
  testWidgets('logo marka adını ve pusula ikonunu gösterir', (tester) async {
    await tester.pumpWidget(const MaterialApp(home: Scaffold(body: AppLogo())));

    expect(find.text('Genç Pusula'), findsOneWidget);
    expect(find.byIcon(Icons.explore_rounded), findsOneWidget);
  });
}
