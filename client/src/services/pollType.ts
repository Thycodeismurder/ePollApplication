export type Poll = {
  Id: number;
  Title: string;
  PollId: number;
  Options: [
    {
      id: number;
      title: string;
      votes: number;
    }
  ];
};
