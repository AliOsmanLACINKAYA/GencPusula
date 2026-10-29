String? validateRequired(String? value, {String message = 'Bu alan gerekli'}) {
  if (value == null || value.trim().isEmpty) return message;
  return null;
}

String? validateEmail(String? value) {
  final email = value?.trim() ?? '';
  if (email.isEmpty) return 'E-posta gerekli';
  final regex = RegExp(r'^[^@\s]+@[^@\s]+\.[^@\s]+$');
  if (!regex.hasMatch(email)) return 'Geçerli bir e-posta gir';
  return null;
}

String? validatePassword(String? value) {
  if (value == null || value.isEmpty) return 'Parola gerekli';
  if (value.length < 6) return 'Parola en az 6 karakter olmalı';
  return null;
}
