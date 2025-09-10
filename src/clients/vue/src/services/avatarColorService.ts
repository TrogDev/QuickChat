const avatarColors = [
  '#FFA94D',
  '#2ECC71',
  '#C77DFF',
  '#FF8E72',
  '#FF6B6B',
  '#00C9A7',
  '#F6A623',
  '#3498DB',
  '#F77FBE',
  '#6BCB77',
  '#E67E22',
  '#17A2B8',
  '#81E979',
  '#8E44AD',
  '#FFD93D',
  '#A3D9FF',
  '#D81159',
  '#FF5D8F',
  '#4D96FF',
  '#6A4C93',
];

export function getParticipantAvatarColor(index: number): string {
  return avatarColors[index % avatarColors.length];
}
